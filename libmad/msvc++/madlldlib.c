/****************************************************************************
 * madlldlib                                                                *
 *                                                                          *
 * Edited version of madlldlib - that works with streams instead of files   *
 * Support for 8 and 24 bits added.                                         *
 * Only mono.                                                               *
 *                                                                          *
 * ---Original code comment from madlldlib below----                        *
 ****************************************************************************
 * madlldlib                                        (c) 2004 J. A. Robson   *
 *                                                                          *
 * madlldlib.cpp -- a library based on madlld that creates a DLL with an    *
 * easy interface--a single function with a callback. (Also changed output  * 
 * bit order to be little-endian.)                                          *
 *                                                                          *
 * How to use:                                                              *
 * 	Define a callback function matching the parameters defined in the       *
 * 	CbMpegAudioDecoder() parameter list. Call said function passing file    *
 * 	path/names and the callback. See inline function comments for           *
 * 	additional details.  See 'test.cpp' for a full example.                 *
 *                                                                          *
 * 	Example:                                                                *
 * 	                                                                        *
 * 	Status=CbMpegAudioDecoder("in.mp3","out.pcm", 0, CallbackFunction);     *
 *                                                                          *
 *  To compile, see Makefile comments.                                      *
 *                                                                          *
 *                                                                          *
 * ---Original code comment below----                                       *
 ****************************************************************************
 * [from madlld.c] (c) 2001--2004 Bertrand Petit							*
 *																			*
 * Redistribution and use in source and binary forms, with or without		*
 * modification, are permitted provided that the following conditions		*
 * are met:																	*
 *																			*
 * 1. Redistributions of source code must retain the above copyright		*
 *    notice, this list of conditions and the following disclaimer.			*
 *																			*
 * 2. Redistributions in binary form must reproduce the above				*
 *    copyright notice, this list of conditions and the following			*
 *    disclaimer in the documentation and/or other materials provided		*
 *    with the distribution.												*
 * 																			*
 * 3. Neither the name of the author nor the names of its contributors		*
 *    may be used to endorse or promote products derived from this			*
 *    software without specific prior written permission.					*
 * 																			*
 * THIS SOFTWARE IS PROVIDED BY THE AUTHOR AND CONTRIBUTORS ``AS IS''		*
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED		*
 * TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A			*
 * PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE AUTHOR OR		*
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,				*
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT			*
 * LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF			*
 * USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND		*
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,		*
 * OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT		*
 * OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF		*
 * SUCH DAMAGE.																*
 *																			*
 ****************************************************************************/


/*
 * $Name:  $
 * $Date: 2004/07/12 21:41:33 $
 * $Revision: 1.7 $
 */

/****************************************************************************
 * Includes																	*
 ****************************************************************************/
#include "madlldlib.h"
#include <stdlib.h>

/****************************************************************************
 * 'Borrowed' from audio.c in the madplay source
 * NAME:	Prng()
 * DESCRIPTION:	32-bit pseudo-random number generator
 ****************************************************************************/
__inline unsigned long Prng(unsigned long state)
{
  return (state * 0x0019660dL + 0x3c6ef35fL) & 0xffffffffL;
}


/****************************************************************************
 * A little more advanced scaling routine based on madplay's 
 * audio_linear_dither() function. Rather than simply rounding the 24 bit
 * number down to 16 bits (as MadFixedToSshort() in madlld does), it performs 
 * dithering, which is the addition of a random number to the least 
 * significant bits (LSB) of the sample that targets the LSB at the 16 bit
 * mark. CbMpegAudioDecoder() uses this to perform its scaling before output.
 ****************************************************************************/
__inline signed int Scale(mad_fixed_t sample, mad_fixed_t *gRandom, int bits)
{
	unsigned int scalebits;
	mad_fixed_t output, mask, rnd;

	/* bias */
	output = sample + (1L << (MAD_F_FRACBITS + 1 - bits - 1));	
	
  	scalebits = MAD_F_FRACBITS + 1 - bits;
  	mask = (1L << scalebits) - 1;

  	/* dither */
	rnd = Prng(*gRandom);
  	output += (rnd & mask) - (*gRandom & mask);
	*gRandom = rnd;
	
  	/* clip */
	if (output >= MAD_F_ONE)
    	output = MAD_F_ONE - 1;
	else if (output < -MAD_F_ONE)
    	output = -MAD_F_ONE;

  	/* quantize */
  	output &= ~mask;

	/* scale */
	return output >> scalebits;
}


#define INPUT_BUFFER_SIZE	(5*8192)
__declspec(dllexport) struct mpeg_decoder_state * __stdcall mpeg_decoder_init(
	int bits,
	int (__stdcall *read_func)(unsigned char * dest, int offset, int dest_size),
	int (__stdcall *is_eof)()
	) {
	struct mpeg_decoder_state *state;
	
	if (bits != 8 && bits != 16 && bits != 24) {
		return NULL;
	}

	state = (struct mpeg_decoder_state *)malloc(sizeof(struct mpeg_decoder_state));

	if (state == NULL) {
		return NULL;
	}

	state->InputBuffer = (unsigned char *)malloc(INPUT_BUFFER_SIZE+MAD_BUFFER_GUARD);
	if (state->InputBuffer == NULL) {
		free(state);
		return NULL;
	}

	state->bits = bits;

	/* Random number for generating dither. 
	 * Used by Scale() below in preparing
	 * the samples for 16 bit output. 
	 */
	state->gRand = 0xa8b9ff7e;
	
	state->read_func = read_func;
	state->is_eof = is_eof;
	
	/* First the structures used by libmad must be initialized. */
	mad_stream_init(&state->Stream);
	mad_frame_init(&state->Frame);
	mad_synth_init(&state->Synth);
	mad_timer_reset(&state->Timer);

	state->pcmPos = state->Synth.pcm.length;
	state->lastValue = -1;
	state->lastValuePos = 0;
	state->FrameCount = 0;

	return state;
}
/****************************************************************************
 * This is based on the MpegAudioDecoder() function in the madlld source.
 * It differs in that it performs the file opens, taking the path/filenames 
 * as arguments, and passes the following information to a callback 
 * function while in the decoding loop, to be handled by the calling code 
 * (generally used for reporting--see test.cpp for an example):
 *
 *			Frame count			
 *			Total input bytes read
 *			A pointer to a mad_header struct (MP3 info)
 *
 * Since this is used as a library function, errors and information are 
 * reported back to a char pointer (StatMsg) passed as a parameter from the 
 * calling code.
 * 
 * It also uses a flag (WavPad) passed from the calling code which tells it
 * whether to add a RIFF/WAVE header (making OutFN a WAV file). If WavPad==1
 * then 44 null bytes are written to the beginning of OutFN before decoding 
 * occurs, and the WAV header is constructed and added after decoding.
 ****************************************************************************/
#define OUTPUT_BUFFER_SIZE	8192 /* Must be an integer multiple of 4. */
__declspec(dllexport) int __stdcall mpeg_decoder_process(
		struct mpeg_decoder_state * state,
		unsigned char * dest, 
		int offset,
		int dest_size		
		) 
{

	unsigned char 		*OutputBuffer = dest+offset,
						*OutputPtr=OutputBuffer,
						*GuardPtr=NULL;
	const unsigned char	*OutputBufferEnd=OutputBuffer+dest_size;
	int					Status=0;
	unsigned long		ByteCount=0;			/* count total input bytes */

	/* This is the decoding loop. */
	while (OutputPtr < OutputBufferEnd)
	{
		
		if (state->lastValue != -1) {
			
			for (; state->lastValuePos < state->bits; state->lastValuePos += 8) {
				if(OutputPtr==OutputBufferEnd)
				{
					// we will continue next time...
					break;
				}

				*(OutputPtr++)=((state->lastValue>>state->lastValuePos) & 0xff);
			}
			state->lastValue = -1;
		}

		/* Synthesized samples must be converted from libmad's fixed
		 * point number to the consumer format. Here we use unsigned
		 * 16 bit big endian integers on two channels. Integer samples
		 * are temporarily stored in a buffer that is flushed when
		 * full.
		 */
		for(;state->pcmPos<state->Synth.pcm.length;state->pcmPos++)
		{
			signed int	Sample;

			if(OutputPtr==OutputBufferEnd)
			{
				break;
			}

			/* Left channel */
			Sample=Scale(state->Synth.pcm.samples[0][state->pcmPos], &state->gRand, state->bits);
			state->lastValuePos = 0;

			/* output in 16 bit little-endian */			
			/*
			*(OutputPtr++)=Sample>>8;       //Originally big-endian
			*(OutputPtr++)=Sample&0xff;
			*/
			for (; state->lastValuePos < state->bits; state->lastValuePos += 8) {
				if(OutputPtr==OutputBufferEnd)
				{
					// Save that value, we will right it next time
					state->lastValue = Sample;
					state->pcmPos++;
					break;
				}

				*(OutputPtr++)=((Sample>>state->lastValuePos) & 0xff);
			}

			if (state->lastValue != -1)
			{
				break;
			}
		}
		
		if(OutputPtr==OutputBufferEnd)
		{
			break;
		}

		/* The input bucket must be filled if it becomes empty or if
		 * it's the first execution of the loop.
		 */
		if(state->Stream.buffer==NULL || state->Stream.error==MAD_ERROR_BUFLEN)
		{
			size_t			ReadSize,
							Remaining;
			unsigned char	*ReadStart;

			/* {2} libmad may not consume all bytes of the input
			 * buffer. If the last frame in the buffer is not wholly
			 * contained by it, then that frame's start is pointed by
			 * the next_frame member of the Stream structure. This
			 * common situation occurs when mad_frame_decode() fails,
			 * sets the stream error code to MAD_ERROR_BUFLEN, and
			 * sets the next_frame pointer to a non NULL value. (See
			 * also the comment marked {4} bellow.)
			 *
			 * When this occurs, the remaining unused bytes must be
			 * put back at the beginning of the buffer and taken in
			 * account before refilling the buffer. This means that
			 * the input buffer must be large enough to hold a whole
			 * frame at the highest observable bit-rate (currently 448
			 * kb/s). XXX=XXX Is 2016 bytes the size of the largest
			 * frame? (448000*(1152/32000))/8
			 */
			if(state->Stream.next_frame!=NULL)
			{
				Remaining=state->Stream.bufend-state->Stream.next_frame;
				memmove(state->InputBuffer,state->Stream.next_frame,Remaining);
				ReadStart=state->InputBuffer+Remaining;
				ReadSize=INPUT_BUFFER_SIZE-Remaining;
			}
			else
				ReadSize=INPUT_BUFFER_SIZE,
					ReadStart=state->InputBuffer,
					Remaining=0;

			/* Fill-in the buffer. If an error occurs print a message
			 * and leave the decoding loop. If the end of stream is
			 * reached we also leave the loop but the return status is
			 * left untouched.
			 */
			ReadSize=state->read_func(ReadStart,0,ReadSize);
			ByteCount+=ReadSize;		/* passed to CbFunc */
			if(ReadSize<=0)
			{
				// End of stream
				break;
			}

			/* {3} When decoding the last frame of a file, it must be
			 * followed by MAD_BUFFER_GUARD zero bytes if one wants to
			 * decode that last frame. When the end of file is
			 * detected we append that quantity of bytes at the end of
			 * the available data. Note that the buffer can't overflow
			 * as the guard size was allocated but not used the the
			 * buffer management code. (See also the comment marked
			 * {1}.)
			 *
			 * In a message to the mad-dev mailing list on May 29th,
			 * 2001, Rob Leslie explains the guard zone as follows:
			 *
			 *    "The reason for MAD_BUFFER_GUARD has to do with the
			 *    way decoding is performed. In Layer III, Huffman
			 *    decoding may inadvertently read a few bytes beyond
			 *    the end of the buffer in the case of certain invalid
			 *    input. This is not detected until after the fact. To
			 *    prevent this from causing problems, and also to
			 *    ensure the next frame's main_data_begin pointer is
			 *    always accessible, MAD requires MAD_BUFFER_GUARD
			 *    (currently 8) bytes to be present in the buffer past
			 *    the end of the current frame in order to decode the
			 *    frame."
			 */
			if(state->is_eof())
			{
				GuardPtr=ReadStart+ReadSize;
				memset(GuardPtr,0,MAD_BUFFER_GUARD);
				ReadSize+=MAD_BUFFER_GUARD;
			}

			/* Pipe the new buffer content to libmad's stream decoder
             * facility.
			 */
			mad_stream_buffer(&state->Stream,state->InputBuffer,ReadSize+Remaining);
			state->Stream.error=(enum mad_error)0;
		}

		/* Decode the next MPEG frame. The streams is read from the
		 * buffer, its constituents are break down and stored the the
		 * Frame structure, ready for examination/alteration or PCM
		 * synthesis. Decoding options are carried in the Frame
		 * structure from the Stream structure.
		 *
		 * Error handling: mad_frame_decode() returns a non zero value
		 * when an error occurs. The error condition can be checked in
		 * the error member of the Stream structure. A mad error is
		 * recoverable or fatal, the error status is checked with the
		 * MAD_RECOVERABLE macro.
		 *
		 * {4} When a fatal error is encountered all decoding
		 * activities shall be stopped, except when a MAD_ERROR_BUFLEN
		 * is signaled. This condition means that the
		 * mad_frame_decode() function needs more input to complete
		 * its work. One shall refill the buffer and repeat the
		 * mad_frame_decode() call. Some bytes may be left unused at
		 * the end of the buffer if those bytes forms an incomplete
		 * frame. Before refilling, the remaining bytes must be moved
		 * to the beginning of the buffer and used for input for the
		 * next mad_frame_decode() invocation. (See the comments
		 * marked {2} earlier for more details.)
		 *
		 * Recoverable errors are caused by malformed bit-streams, in
		 * this case one can call again mad_frame_decode() in order to
		 * skip the faulty part and re-sync to the next frame.
		 */
		if(mad_frame_decode(&state->Frame,&state->Stream))
		{
			if(MAD_RECOVERABLE(state->Stream.error))
			{
				continue;
			}
			else
				if(state->Stream.error==MAD_ERROR_BUFLEN)
					continue;
				else
				{
					Status=1;
					break;
				}
		}


		/* Accounting. The computed frame duration is in the frame
		 * header structure. It is expressed as a fixed point number
		 * whole data type is mad_timer_t. It is different from the
		 * samples fixed point format and unlike it, it can't directly
		 * be added or subtracted. The timer module provides several
		 * functions to operate on such numbers. Be careful there, as
		 * some functions of libmad's timer module receive some of
		 * their mad_timer_t arguments by value!
		 */
		state->FrameCount++;
		

		mad_timer_add(&state->Timer,state->Frame.header.duration);	

		
		/* Once decoded the frame is synthesized to PCM samples. No errors
		 * are reported by mad_synth_frame();
		 */
		mad_synth_frame(&state->Synth,&state->Frame);
		state->pcmPos = 0;
	}

	/* That's the end of the world (in the H. G. Wells way). */
	if (Status == 0) {
		return OutputPtr - OutputBuffer;
	}
	return Status;
}


__declspec(dllexport) void  __stdcall mpeg_decoder_destroy(struct mpeg_decoder_state * state) 
{
	if (state != NULL) {
		mad_synth_finish(&state->Synth);
		mad_frame_finish(&state->Frame);
		mad_stream_finish(&state->Stream);

		if (state->InputBuffer != NULL) {
			free(state->InputBuffer);
		}

		free(state);
	}
}


/****************************************************************************
 * End of file madlldlib.cpp                                                *
 ****************************************************************************/