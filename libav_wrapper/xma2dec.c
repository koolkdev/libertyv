#define inline __inline

#include "xma2dec.h"

#include <libavutil/common.h>
#include <stdio.h>
#include <string.h>

int avcodec_initialized = 0;

xma2_dec_context * xma2_dec_init(int sample_rate, int channels, int bits) {
    AVCodec *codec;
	xma2_dec_context *ctx;

	if (!avcodec_initialized) {
		avcodec_register_all();
		avcodec_initialized = 1;
	}

	if (bits <= 0 || bits > 32 || (bits % 8) != 0) {
		return NULL;
	}
	
	ctx = malloc(sizeof(xma2_dec_context));
	if (!ctx) {
		return NULL;
	}

	memset(ctx, 0, sizeof(xma2_dec_context));

	ctx->bits = bits;
	ctx->current_frame_pos = 0;

	ctx->avpkt.data = ctx->packet;
	ctx->avpkt.size = 0;

	ctx->frame_samples_size = 0;
	ctx->offset = 0;

	// maximum size of the buffer
	ctx->current_frame = malloc(SAMPLES_PER_FRAME * channels * (bits / 8));
	if (!ctx->current_frame) {
		xma2_dec_free(ctx);
		return NULL;
	}

	ctx->decoded_frame = avcodec_alloc_frame();
	if (!ctx->decoded_frame) {
		xma2_dec_free(ctx);
		return NULL;

	}

	// Warning: This lib works with a modifided version of ffmpeg
    codec = avcodec_find_decoder(AV_CODEC_ID_WMAPRO);
	if (!codec) {
		xma2_dec_free(ctx);
		return NULL;
	}

	ctx->codec_ctx = avcodec_alloc_context3(codec);
	if (!ctx->codec_ctx) {
		xma2_dec_free(ctx);
		return NULL;
	}
	        
    ctx->codec_ctx->channels    = channels;
    ctx->codec_ctx->sample_rate = sample_rate;
    //ctx->codec_ctx->bit_rate    = avctx->bit_rate;
	ctx->codec_ctx->block_align = PACKET_SIZE;
    
    ctx->codec_ctx->extradata_size = 18;
    ctx->codec_ctx->extradata = malloc(18);
    if (!ctx->codec_ctx->extradata) {
		xma2_dec_free(ctx);
        return NULL;
	}
        
    *(short*)(ctx->codec_ctx->extradata) = 0x10; // bits per sample
    *(int*)(ctx->codec_ctx->extradata+2) = 1; // channel mask
    *(short*)(ctx->codec_ctx->extradata+14) = 0x10D6; // decode flags

    /* open it */
    if (avcodec_open2(ctx->codec_ctx, codec, NULL) < 0) {
		xma2_dec_free(ctx);
        return NULL;
	}
	
	return ctx;
}

// Input must be 0x800 bytes
int xma2_dec_prepare_packet(xma2_dec_context * ctx, unsigned char * input, int input_size) {
	if (input_size != PACKET_SIZE) {
		// must get a full packet only
		return -1;
	}
	if (ctx->avpkt.size > 0 || ctx->current_frame_pos != ctx->frame_samples_size) {
		// There is already unfinished packet
		return -1;
	}
	memcpy(ctx->packet, input, PACKET_SIZE);
	ctx->avpkt.data = ctx->packet;
    ctx->avpkt.size = PACKET_SIZE;
	// make the packet header wmapro-compatible
    *((int*)ctx->avpkt.data) = (((ctx->offset & 0x7800) | 0x400) >> 7) | (*((int*)ctx->avpkt.data) & 0xFFFEFF08);
	ctx->offset += 0x800;

	return 0;
}

// read the decoded packet
int xma2_dec_read(xma2_dec_context * ctx, unsigned char * output, int output_offset, int output_size) {
	int to_copy;
	int original_offset = output_offset;
	int sample_size = ctx->bits / 8;
	// We already have a packet
	if (ctx->current_frame_pos != ctx->frame_samples_size) {
		to_copy = min(output_size, ctx->frame_samples_size - ctx->current_frame_pos);
		memcpy(output + output_offset, ctx->current_frame + ctx->current_frame_pos, to_copy);

		ctx->current_frame_pos += to_copy;
		output_size -= to_copy;
		output_offset += to_copy;
	}

	while (output_size > 0 && ctx->avpkt.size > 0) {
        int got_frame = 0;
		int len = 0;
        avcodec_get_frame_defaults(ctx->decoded_frame);
		len = avcodec_decode_audio4(ctx->codec_ctx, ctx->decoded_frame, &got_frame, &ctx->avpkt);
        if (len < 0) {
			// error in codec
			return -1;
        }
        ctx->avpkt.size -= len;
        ctx->avpkt.data += len;
        ctx->avpkt.dts =
        ctx->avpkt.pts = AV_NOPTS_VALUE;
        if (got_frame) {
			int i;
            /* if a frame has been decoded, output it */
			float * curSample = (float *)ctx->decoded_frame->data[0];

			if (ctx->decoded_frame->nb_samples > SAMPLES_PER_FRAME) {
				// that is bad
				return -1;
			}
			if (ctx->codec_ctx->sample_fmt != AV_SAMPLE_FMT_FLTP) {
				// that is bad
				return -1;
			}

			if (av_samples_get_buffer_size(NULL, ctx->codec_ctx->channels, ctx->decoded_frame->nb_samples,  ctx->codec_ctx->sample_fmt, 1) != ctx->codec_ctx->channels * ctx->decoded_frame->nb_samples * sizeof(float)) {
				// that is bad also
				return -1;
			}

			ctx->frame_samples_size = ctx->codec_ctx->channels * ctx->decoded_frame->nb_samples * sample_size;

			for (i = 0; i < ctx->codec_ctx->channels * ctx->decoded_frame->nb_samples; ++i) {
				float fsample = curSample[i] * (1<<(ctx->bits -1));
				int sample, j;
				// Wierd problem: the sample should be in range -1, 1. but it isn't the case. it is bigger sometimes..
				if (fsample >= 0) {
					fsample += 0.5;
					if (fsample > (1<<(ctx->bits - 1)) - 1) {
						fsample = (float)(1<<(ctx->bits - 1)) - 1;
					}
				} else {
					fsample -= 0.5;
				}
				sample = (int)fsample;
				for (j = 0; j < sample_size; ++j) {
					ctx->current_frame[i * sample_size + j] = sample & 0xff;
					sample >>= 8;
				}
			}
			ctx->current_frame_pos = 0;

			to_copy = min(output_size, ctx->frame_samples_size - ctx->current_frame_pos);
			memcpy(output + output_offset, ctx->current_frame + ctx->current_frame_pos, to_copy);

			ctx->current_frame_pos += to_copy;
			output_size -= to_copy;
			output_offset += to_copy;

        }
    }

	return output_offset - original_offset;
}

void xma2_dec_free(xma2_dec_context * ctx) {
	if (ctx != NULL) {
		if (ctx->codec_ctx != NULL) {
			if (ctx->codec_ctx->extradata != NULL) {
				free(ctx->codec_ctx->extradata);
				ctx->codec_ctx->extradata = NULL;
			}
			if (avcodec_is_open(ctx->codec_ctx)) {
				avcodec_close(ctx->codec_ctx);
			}
			av_free(ctx->codec_ctx);
			ctx->codec_ctx = NULL;
		}
		if (ctx->decoded_frame != NULL) {
			avcodec_free_frame(&ctx->decoded_frame);
			ctx->decoded_frame= NULL;
		}
		if (ctx->current_frame != NULL) {
			free(ctx->current_frame);
			ctx->current_frame = NULL;
		}
		free(ctx);
	}
}