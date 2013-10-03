#ifndef __MP3DEC_H__
#define __MP3DEC_H__

#include <libavcodec/avcodec.h>

#define INPUT_BUFFER_SIZE (8192)
#define PACKET_SIZE (1024)
#define SAMPLES_PER_FRAME (1152)

typedef struct {
	AVCodecContext * codec_ctx;
	AVPacket avpkt;
	AVFrame *decoded_frame;
	unsigned char frames[INPUT_BUFFER_SIZE];
	int frame_samples_size;
	unsigned char * current_frame;
	int current_frame_pos;
	int bits;
	int (__stdcall *read_func)(unsigned char * dest, int offset, int dest_size);
} mp3_dec_context;

__declspec(dllexport) mp3_dec_context * mp3_dec_init(int (__stdcall *read_func)(unsigned char * dest, int offset, int dest_size), int bits);

__declspec(dllexport) int mp3_dec_read(mp3_dec_context * ctx, unsigned char * output, int output_offset, int output_size);

__declspec(dllexport) void mp3_dec_free(mp3_dec_context * ctx);

#endif