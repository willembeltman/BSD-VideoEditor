using FFmpeg.AutoGen;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.Runtime.InteropServices;
using VideoEditorD3D.FFMpeg.Types;
using Device = SharpDX.Direct3D11.Device;

namespace VideoEditorD3D.FFMpeg.Autogen;

public unsafe class GpuFrameReaderFirst : IDisposable
{
    private readonly AVFormatContext* FormatContext;
    private readonly AVCodecContext* CodecContext;
    private readonly AVBufferRef* HwDeviceCtx;
    private readonly AVFrame* Frame;
    private readonly AVPacket* Packet;

    private readonly Device D3dDevice;
    private readonly DeviceContext D3dContext;

    private int VideoStreamIndex;

    public GpuFrameReaderFirst(string filename, Device d3dDevice)
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

        // Find video stream
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

        AVStream* stream = FormatContext->streams[VideoStreamIndex];
        AVCodec* codec = ffmpeg.avcodec_find_decoder(stream->codecpar->codec_id);
        if (codec == null)
            throw new ApplicationException("Unsupported codec");

        CodecContext = ffmpeg.avcodec_alloc_context3(codec);
        ffmpeg.avcodec_parameters_to_context(CodecContext, stream->codecpar);

        // Initialize HW device context (D3D11)
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
    }

    public IEnumerable<GpuFrame> ReadFrames()
    {
        long frameIndex = 0;
        while (ReadFrame(out var gpuFrame, ref frameIndex))
            if (gpuFrame != null)
                yield return gpuFrame;
    }
    public bool ReadFrame(out GpuFrame? gpuFrame, ref long frameIndex)
    {
        gpuFrame = null;
        if (ffmpeg.av_read_frame(FormatContext, Packet) >= 0)
        {
            if (Packet->stream_index == VideoStreamIndex)
            {
                if (ffmpeg.avcodec_send_packet(CodecContext, Packet) < 0)
                    return true; 

                if (ffmpeg.avcodec_receive_frame(CodecContext, Frame) == 0)
                {
                    // _frame now contains a decoded frame - possibly in hardware surface

                    // Check if it's a hardware frame (D3D11 surface)
                    if (Frame->format == (int)AVPixelFormat.AV_PIX_FMT_D3D11)
                    {
                        // Cast pointer naar echte Texture2D
                        // Frame->data[0] als IntPtr lezen
                        IntPtr resourcePtr = (IntPtr)(*(void**)&Frame->data);

                        // Stap 2: Wrap het als een Texture2D (zonder te ownen)
                        var texture = new Texture2D(resourcePtr); // of via SharpDX.Direct3D11.Resource.FromPointer(resourcePtr)

                        // Compute timestamp
                        double timestamp = Frame->best_effort_timestamp * ffmpeg.av_q2d(FormatContext->streams[VideoStreamIndex]->time_base);

                        var resolution = new Resolution(Frame->width, Frame->height);

                        gpuFrame = new GpuFrame(resolution, texture, frameIndex++, timestamp, false);
                        return true;
                    }
                    else
                    {
                        // fallback: convert software frame to texture, or ignore
                        // (omdat je vraag gericht is op GPU buffers, skip ik deze)
                    }
                }
            }
            ffmpeg.av_packet_unref(Packet);
        }

        return false;
    }

    public unsafe void Dispose()
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