using FFmpeg.AutoGen;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using VideoEditorD3D.FFMpeg.Types;
using Device = SharpDX.Direct3D11.Device;

namespace VideoEditorD3D.FFMpeg.Autogen;

public unsafe class GpuVideoFrameReader : IDisposable
{
    private readonly AVFormatContext* FormatContext;
    private readonly AVCodecContext* CodecContext;
    private readonly AVBufferRef* HwDeviceCtx;
    private readonly AVFrame* Frame;
    private readonly AVPacket* Packet;

    private readonly Device D3dDevice;
    private readonly DeviceContext D3dContext;

    private int VideoStreamIndex;
    private AVStream* VideoStream;

    public GpuVideoFrameReader(string filename, Device d3dDevice, long startFrame = 0)
    {
        D3dDevice = d3dDevice;
        D3dContext = d3dDevice.ImmediateContext;

        ffmpeg.avdevice_register_all();
        ffmpeg.avformat_network_init();

        FormatContext = ffmpeg.avformat_alloc_context();

        fixed (AVFormatContext** formatContextPtr = &FormatContext)
        {
            if (ffmpeg.avformat_open_input(formatContextPtr, filename, null, null) != 0)
                throw new ApplicationException("Could not open video file");
        }

        if (ffmpeg.avformat_find_stream_info(FormatContext, null) != 0)
            throw new ApplicationException("Could not find stream info");

        for (int i = 0; i < FormatContext->nb_streams; i++)
        {
            if (FormatContext->streams[i]->codecpar->codec_type == AVMediaType.AVMEDIA_TYPE_VIDEO)
            {
                VideoStreamIndex = i;
                break;
            }
        }
        if (VideoStreamIndex < 0)
            throw new ApplicationException("No video stream found");

        VideoStream = FormatContext->streams[VideoStreamIndex];

        AVCodec* codec = ffmpeg.avcodec_find_decoder(VideoStream->codecpar->codec_id);
        if (codec == null)
            throw new ApplicationException("Unsupported codec");

        CodecContext = ffmpeg.avcodec_alloc_context3(codec);
        ffmpeg.avcodec_parameters_to_context(CodecContext, VideoStream->codecpar);

        fixed (AVBufferRef** hwDeviceContextPtr = &HwDeviceCtx)
        {
            if (ffmpeg.av_hwdevice_ctx_create(hwDeviceContextPtr, AVHWDeviceType.AV_HWDEVICE_TYPE_D3D11VA, null, null, 0) < 0)
                throw new ApplicationException("Failed to create HW device context");
        }
        CodecContext->hw_device_ctx = ffmpeg.av_buffer_ref(HwDeviceCtx);

        if (ffmpeg.avcodec_open2(CodecContext, codec, null) < 0)
            throw new ApplicationException("Could not open codec");

        Frame = ffmpeg.av_frame_alloc();
        Packet = ffmpeg.av_packet_alloc();

        if (startFrame > 0)
        {
            long seekTarget = ffmpeg.av_rescale_q(startFrame, new AVRational { num = 1, den = (int)VideoStream->r_frame_rate.num }, VideoStream->time_base);
            ffmpeg.av_seek_frame(FormatContext, VideoStreamIndex, seekTarget, ffmpeg.AVSEEK_FLAG_BACKWARD);
            ffmpeg.avcodec_flush_buffers(CodecContext);
        }
    }

    public IEnumerable<GpuVideoFrame> ReadFrames()
    {
        var frameIndex = 0L;
        while (ReadFrame(out var gpuFrame, ref frameIndex))
            if (gpuFrame != null)
                yield return gpuFrame;
    }
    public bool ReadFrame(out GpuVideoFrame? gpuFrame, ref long frameIndex)
    {
        gpuFrame = null;

        while (true)
        {
            int ret = ffmpeg.av_read_frame(FormatContext, Packet);
            if (ret < 0)
            {
                // EOF of fout
                return false;
            }

            try
            {
                if (Packet->stream_index == VideoStreamIndex)
                {
                    ret = ffmpeg.avcodec_send_packet(CodecContext, Packet);
                    if (ret < 0)
                    {
                        throw new ApplicationException($"Error sending packet to decoder: {ret}");
                    }

                    while (true)
                    {
                        ret = ffmpeg.avcodec_receive_frame(CodecContext, Frame);
                        if (ret == 0)
                        {
                            bool isKeyFrame = (Frame->flags & ffmpeg.AV_FRAME_FLAG_KEY) != 0;
                            double clipTime = Frame->best_effort_timestamp * ffmpeg.av_q2d(VideoStream->time_base);
                            var resolution = new Resolution(Frame->width, Frame->height);

                            if (Frame->format == (int)AVPixelFormat.AV_PIX_FMT_D3D11)
                            {
                                gpuFrame = CopyFrameD3D11(ref frameIndex, isKeyFrame, clipTime, resolution);
                            }
                            else
                            {
                                gpuFrame = CopyFrameSoftware(ref frameIndex, isKeyFrame, clipTime, resolution);
                            }

                            return true;
                        }
                        else if (ret == ffmpeg.AVERROR(ffmpeg.EAGAIN))
                        {
                            break; // meer pakketten nodig
                        }
                        else if (ret == ffmpeg.AVERROR_EOF)
                        {
                            return false;
                        }
                        else
                        {
                            throw new ApplicationException($"Error receiving frame from decoder: {ret}");
                        }
                    }
                }
            }
            finally
            {
                // Ongeacht wat er gebeurde, packet unref moet altijd
                ffmpeg.av_packet_unref(Packet);
            }
        }
    }

    private GpuVideoFrame CopyFrameSoftware(ref long frameIndex, bool isKeyFrame, double clipTime, Resolution resolution)
    {
        // Maak nieuw frame aan voor RGB data
        AVFrame* rgbFrame = ffmpeg.av_frame_alloc();

        // Stel width, height en pixelformat in
        rgbFrame->format = (int)AVPixelFormat.AV_PIX_FMT_RGBA;
        rgbFrame->width = Frame->width;
        rgbFrame->height = Frame->height;

        // Allocate buffer voor rgbFrame data
        int ret = ffmpeg.av_frame_get_buffer(rgbFrame, 32); // 32 bytes alignment
        if (ret < 0)
            throw new ApplicationException("Could not allocate RGB frame buffer");

        // Setup SwsContext voor conversie YUV -> RGBA
        SwsContext* swsCtx = ffmpeg.sws_getContext(
            Frame->width, Frame->height, (AVPixelFormat)Frame->format,
            rgbFrame->width, rgbFrame->height, AVPixelFormat.AV_PIX_FMT_RGBA,
            ffmpeg.SWS_BILINEAR, null, null, null);

        if (swsCtx == null)
            throw new ApplicationException("Could not initialize sws context");

        // Converteer frame
        ffmpeg.sws_scale(
            swsCtx,
            Frame->data,
            Frame->linesize,
            0, 
            Frame->height,
            rgbFrame->data,
            rgbFrame->linesize);

        // SwsContext vrijgeven
        ffmpeg.sws_freeContext(swsCtx);

        // 2. Maak een Texture2D aan op GPU met juiste formaat
        var textureDesc = new Texture2DDescription
        {
            Width = rgbFrame->width,
            Height = rgbFrame->height,
            Format = Format.R8G8B8A8_UNorm,
            MipLevels = 1,
            ArraySize = 1,
            Usage = ResourceUsage.Default,
            BindFlags = BindFlags.ShaderResource,
            CpuAccessFlags = CpuAccessFlags.None,
            SampleDescription = new SampleDescription(1, 0),
            OptionFlags = ResourceOptionFlags.None,
        };

        var texture = new Texture2D(D3dDevice, textureDesc);

        // 3. Upload data naar texture
        D3dContext.UpdateSubresource(new DataBox((IntPtr)rgbFrame->data[0], rgbFrame->linesize[0], 0), texture, 0);

        var gpuFrame = new GpuVideoFrame(resolution, texture, frameIndex++, clipTime, isKeyFrame);

        // Vergeet rgbFrame niet te free'en
        ffmpeg.av_frame_free(&rgbFrame);

        return gpuFrame;
    }

    private GpuVideoFrame CopyFrameD3D11(ref long frameIndex, bool isKeyFrame, double clipTime, Resolution resolution)
    {
        GpuVideoFrame gpuFrame;
        var resourcePtr = (IntPtr)(*(void**)&Frame->data);
        var wrappedResource = new Texture2D(resourcePtr);

        // Maak een echte kopie zodat we kunnen disposen
        var desc = wrappedResource.Description;
        var copiedTexture = new Texture2D(D3dDevice, desc);
        D3dContext.CopyResource(wrappedResource, copiedTexture);

        gpuFrame = new GpuVideoFrame(resolution, copiedTexture, frameIndex++, clipTime, isKeyFrame);
        return gpuFrame;
    }

    public void Dispose()
    {
        fixed (AVFrame** frame = &Frame)
        fixed (AVPacket** packet = &Packet)
        fixed (AVCodecContext** codecContext = &CodecContext)
        fixed (AVFormatContext** formatContext = &FormatContext)
        fixed (AVBufferRef** hwDeviceCtx = &HwDeviceCtx)
        {
            ffmpeg.av_frame_free(frame);
            ffmpeg.av_packet_free(packet);
            ffmpeg.avcodec_free_context(codecContext);
            ffmpeg.avformat_close_input(formatContext);
            ffmpeg.av_buffer_unref(hwDeviceCtx);
        }

        GC.SuppressFinalize(this);
    }
}
