//using FFmpeg.AutoGen;
//using SharpDX.Direct3D11;
//using SharpDX.DXGI;
//using System;
//using System.Collections.Generic;
//using System.Drawing.Imaging;
//using System.Runtime.InteropServices;

//public unsafe class GPUFrame
//{
//    public Texture2D Texture { get; }
//    public long FrameIndex { get; }
//    public double Timestamp { get; }

//    public GPUFrame(Texture2D texture, long frameIndex, double timestamp)
//    {
//        Texture = texture;
//        FrameIndex = frameIndex;
//        Timestamp = timestamp;
//    }
//}

//public unsafe class FFmpegGpuFrameReader : IDisposable
//{
//    private AVFormatContext* _formatContext;
//    private AVCodecContext* _codecContext;
//    private AVBufferRef* _hwDeviceCtx;
//    private AVFrame* _frame;
//    private AVPacket* _packet;

//    private Device _d3dDevice;
//    private DeviceContext _d3dContext;

//    private int _videoStreamIndex;

//    public FFmpegGpuFrameReader(string filename, Device d3dDevice)
//    {
//        _d3dDevice = d3dDevice;
//        _d3dContext = d3dDevice.ImmediateContext;

//        ffmpeg.av_register_all();
//        ffmpeg.avformat_network_init();

//        _formatContext = ffmpeg.avformat_alloc_context();

//        if (ffmpeg.avformat_open_input(&_formatContext, filename, null, null) != 0)
//            throw new ApplicationException("Could not open video file");

//        if (ffmpeg.avformat_find_stream_info(_formatContext, null) != 0)
//            throw new ApplicationException("Could not find stream info");

//        // Find video stream
//        for (int i = 0; i < _formatContext->nb_streams; i++)
//        {
//            if (_formatContext->streams[i]->codecpar->codec_type == AVMediaType.AVMEDIA_TYPE_VIDEO)
//            {
//                _videoStreamIndex = i;
//                break;
//            }
//        }
//        if (_videoStreamIndex < 0)
//            throw new ApplicationException("No video stream found");

//        AVStream* stream = _formatContext->streams[_videoStreamIndex];
//        AVCodec* codec = ffmpeg.avcodec_find_decoder(stream->codecpar->codec_id);
//        if (codec == null)
//            throw new ApplicationException("Unsupported codec");

//        _codecContext = ffmpeg.avcodec_alloc_context3(codec);
//        ffmpeg.avcodec_parameters_to_context(_codecContext, stream->codecpar);

//        // Initialize HW device context (D3D11)
//        if (ffmpeg.av_hwdevice_ctx_create(&_hwDeviceCtx, AVHWDeviceType.AV_HWDEVICE_TYPE_D3D11VA, null, null, 0) < 0)
//            throw new ApplicationException("Failed to create HW device context");

//        _codecContext->hw_device_ctx = ffmpeg.av_buffer_ref(_hwDeviceCtx);

//        if (ffmpeg.avcodec_open2(_codecContext, codec, null) < 0)
//            throw new ApplicationException("Could not open codec");

//        _frame = ffmpeg.av_frame_alloc();
//        _packet = ffmpeg.av_packet_alloc();
//    }

//    public IEnumerable<GPUFrame> ReadFrames()
//    {
//        long frameIndex = 0;
//        while (ffmpeg.av_read_frame(_formatContext, _packet) >= 0)
//        {
//            if (_packet->stream_index == _videoStreamIndex)
//            {
//                if (ffmpeg.avcodec_send_packet(_codecContext, _packet) < 0)
//                    continue;

//                while (ffmpeg.avcodec_receive_frame(_codecContext, _frame) == 0)
//                {
//                    // _frame now contains a decoded frame - possibly in hardware surface

//                    // Check if it's a hardware frame (D3D11 surface)
//                    if (_frame->format == (int)AVPixelFormat.AV_PIX_FMT_D3D11)
//                    {
//                        // Get the underlying D3D11 texture pointer from frame->data[0]
//                        var texturePtr = *(IntPtr*)_frame->data;

//                        // Wrap it as a SharpDX Texture2D
//                        var texture = new Texture2D(_d3dDevice, texturePtr);

//                        // Compute timestamp
//                        double timestamp = _frame->best_effort_timestamp * ffmpeg.av_q2d(_formatContext->streams[_videoStreamIndex]->time_base);

//                        yield return new GPUFrame(texture, frameIndex++, timestamp);
//                    }
//                    else
//                    {
//                        // fallback: convert software frame to texture, or ignore
//                        // (omdat je vraag gericht is op GPU buffers, skip ik deze)
//                    }
//                }
//            }
//            ffmpeg.av_packet_unref(_packet);
//        }
//    }

//    public void Dispose()
//    {
//        ffmpeg.av_frame_free(&_frame);
//        ffmpeg.av_packet_free(&_packet);
//        ffmpeg.avcodec_free_context(&_codecContext);
//        ffmpeg.avformat_close_input(&_formatContext);
//        ffmpeg.av_buffer_unref(&_hwDeviceCtx);
//    }
//}