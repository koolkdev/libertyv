#ifndef __XMA2DEC_H__
#define __XMA2DEC_H__

#include <libavcodec/avcodec.h>

#define PACKET_SIZE (0x800)
#define SAMPLES_PER_FRAME (0x200)

typedef struct {
	AVCodecContext * codec_ctx;
	AVPacket avpkt;
	AVFrame *decoded_frame;
	unsigned char packet[PACKET_SIZE];
	int offset;
	int frame_samples_size;
	unsigned char * current_frame;
	int current_frame_pos;
	int bits;
} xma2_dec_context;

__declspec(dllexport) xma2_dec_context * xma2_dec_init(int sample_rate, int channels, int bits);

// Input must be 0x800 bytes
__declspec(dllexport) int xma2_dec_prepare_packet(xma2_dec_context * ctx, unsigned char * input, int input_size);

// read the decoded packet
__declspec(dllexport) int xma2_dec_read(xma2_dec_context * ctx, unsigned char * output, int output_offset, int output_size);

__declspec(dllexport) void xma2_dec_free(xma2_dec_context * ctx);

#endif