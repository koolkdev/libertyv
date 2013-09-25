/****************************************************************************
 * madlldlib                                        (c) 2004 J. A. Robson   *
 *																			*
 * Definitions file. See inline comments for details. See comments in       *
 * source files 'madlldlib.cpp' and 'test.cpp' for information on           *
 * programming madlldlib. To compile, see Makefile comments.                *
 *																			*
 ****************************************************************************/

#ifndef _MADLLDLIB_H_
#define _MADLLDLIB_H_

/* program includes */
#include <mad.h> /* must include this path in compile */

struct mpeg_decoder_state {
	struct mad_stream	Stream;
	struct mad_frame	Frame;
	struct mad_synth	Synth;
	mad_timer_t			Timer;
	int					pcmPos;
	signed int			lastValue;
	int					lastValuePos;
	int					bits;
	unsigned int		FrameCount;
	unsigned char *		InputBuffer;
	int (__stdcall *read_func)(unsigned char * dest, int offset, int dest_size);
	int (__stdcall *is_eof)();
	mad_fixed_t gRand;
};

/* function export */
__declspec(dllexport) struct mpeg_decoder_state * __stdcall mpeg_decoder_init(
	int bits,
	int (__stdcall *read_func)(unsigned char * dest, int offset, int dest_size),
	int (__stdcall *is_eof)()
	);


/* callback decoder function (based on MpegAudioDecoder) */
__declspec(dllexport) int __stdcall mpeg_decoder_process(
		struct mpeg_decoder_state * state,
		unsigned char * dest, 
		int offset,
		int dest_size		
		);

__declspec(dllexport) void  __stdcall mpeg_decoder_destroy(struct mpeg_decoder_state * state);

#endif

