#define inline __inline

#include "mp3dec.h"

#include <libavutil/common.h>
#include <stdio.h>
#include <string.h>

extern int avcodec_initialized;

mp3_dec_context * mp3_dec_init(int (__stdcall *read_func)(unsigned char * dest, int offset, int dest_size), int bits) {
    AVCodec *codec;
	mp3_dec_context *ctx;

	if (!avcodec_initialized) {
		avcodec_register_all();
		avcodec_initialized = 1;
	}

	// right now only 16 bits are supported
	if (bits != 16) {
		return NULL;
	}
	
	ctx = malloc(sizeof(mp3_dec_context));
	if (!ctx) {
		return NULL;
	}

	memset(ctx, 0, sizeof(mp3_dec_context));

	ctx->read_func = read_func;

	ctx->bits = bits;
	ctx->current_frame_pos = 0;

	ctx->avpkt.data = ctx->frames;
	ctx->avpkt.size = 0;

	ctx->frame_samples_size = 0;

	// maximum size of the buffer, maximum 6 channels
	ctx->current_frame = malloc(SAMPLES_PER_FRAME * 6 * (bits / 8));
	if (!ctx->current_frame) {
		mp3_dec_free(ctx);
		return NULL;
	}

	ctx->decoded_frame = avcodec_alloc_frame();
	if (!ctx->decoded_frame) {
		mp3_dec_free(ctx);
		return NULL;

	}

	// Warning: This lib works with a modifided version of ffmpeg
    codec = avcodec_find_decoder(AV_CODEC_ID_MP3);
	if (!codec) {
		mp3_dec_free(ctx);
		return NULL;
	}

	ctx->codec_ctx = avcodec_alloc_context3(codec);
	if (!ctx->codec_ctx) {
		mp3_dec_free(ctx);
		return NULL;
	}
	        
    ctx->codec_ctx->bit_rate    = bits;

    /* open it */
    if (avcodec_open2(ctx->codec_ctx, codec, NULL) < 0) {
		mp3_dec_free(ctx);
        return NULL;
	}
	
	return ctx;
}

// read the decoded packet
int mp3_dec_read(mp3_dec_context * ctx, unsigned char * output, int output_offset, int output_size) {
	int len;
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

	if (output_size > 0 && ctx->avpkt.size < PACKET_SIZE) {
		memmove(ctx->frames, ctx->avpkt.data, ctx->avpkt.size);
		ctx->avpkt.data = ctx->frames;
		len = ctx->read_func(ctx->avpkt.data + ctx->avpkt.size, 0, sizeof(ctx->frames) - ctx->avpkt.size);
		if (len > 0) 
			ctx->avpkt.size += len;
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
            /* if a frame has been decoded, output it */
			if (ctx->decoded_frame->nb_samples > SAMPLES_PER_FRAME) {
				// that is bad
				return -1;
			}
			if (ctx->codec_ctx->sample_fmt != AV_SAMPLE_FMT_S16P) {
				// that is bad
				return -1;
			}

			if (ctx->codec_ctx->channels != 1) {
				// TODO: not supported right now
				return -1;
			}

			if (av_samples_get_buffer_size(NULL, ctx->codec_ctx->channels, ctx->decoded_frame->nb_samples,  ctx->codec_ctx->sample_fmt, 1) != ctx->codec_ctx->channels * ctx->decoded_frame->nb_samples * sample_size) {
				// that is bad also
				return -1;
			}

			ctx->frame_samples_size = ctx->codec_ctx->channels * ctx->decoded_frame->nb_samples * sample_size;

			// TODO: we could give on using current_frame, and keep using decoded_frame instaed
			memcpy(ctx->current_frame, ctx->decoded_frame->data[0], ctx->frame_samples_size);
			ctx->current_frame_pos = 0;

			to_copy = min(output_size, ctx->frame_samples_size - ctx->current_frame_pos);
			memcpy(output + output_offset, ctx->current_frame + ctx->current_frame_pos, to_copy);

			ctx->current_frame_pos += to_copy;
			output_size -= to_copy;
			output_offset += to_copy;
        }

		
		if (output_size > 0 && ctx->avpkt.size < PACKET_SIZE) {
			memmove(ctx->frames, ctx->avpkt.data, ctx->avpkt.size);
			ctx->avpkt.data = ctx->frames;
			len = ctx->read_func(ctx->avpkt.data + ctx->avpkt.size, 0, sizeof(ctx->frames) - ctx->avpkt.size);
			if (len > 0) 
				ctx->avpkt.size += len;
		}
    }

	return output_offset - original_offset;
}

void mp3_dec_free(mp3_dec_context * ctx) {
	if (ctx != NULL) {
		if (ctx->codec_ctx != NULL) {
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