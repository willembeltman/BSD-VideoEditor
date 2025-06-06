﻿using SharpDX.Mathematics.Interop;
using VideoEditorD3D.Direct3D.Controls.Templates;
using VideoEditorD3D.Direct3D.Drawing;
using VideoEditorD3D.Direct3D.Events;
using VideoEditorD3D.Direct3D.Interfaces;

namespace VideoEditorD3D.Direct3D.Controls;

public class HScrollBar : ForeBorderBackControl
{
    public HScrollBar()
    {
        Background = GraphicsLayers.CreateNewLayer();
        Thumb = GraphicsLayers.CreateNewLayer();
        Border = GraphicsLayers.CreateNewLayer();

        BackColor = new RawColor4(0.2f, 0.2f, 0.2f, 1);
        ForeColor = new RawColor4(0.6f, 0.6f, 0.6f, 1);
        BorderColor = new RawColor4(1, 1, 1, 1);
        BorderSize = 1;

        Draw += HScrollBar_Draw;
        MouseDown += HScrollBar_MouseDown;
        MouseMove += HScrollBar_MouseMove;
        MouseUp += HScrollBar_MouseUp;
        MouseLeave += HScrollBar_MouseLeave;
    }


    private readonly GraphicsLayer Background;
    private readonly GraphicsLayer Thumb;
    private readonly GraphicsLayer Border;

    private bool isDragging = false;
    private float dragOffsetX;

    public event EventHandler<ScrollEventArgs>? Scroll;

    private float _Minimum = 0;
    private float _Maximum = 1;
    private float _LargeChange = 0.1f;
    private float _Value = 0;

    public float Minimum
    {
        get => _Minimum;
        set
        {
            if (_Minimum == value) return;
            _Minimum = value;
            Invalidate();
        }
    }
    public float Maximum
    {
        get => _Maximum;
        set
        {
            if (_Maximum == value) return;
            _Maximum = value;
            Invalidate();
        }
    }
    public float LargeChange
    {
        get => _LargeChange;
        set
        {
            if (_LargeChange == value) return;
            _LargeChange = value;
            Invalidate();
        }
    }
    public float Value
    {
        get => _Value;
        set
        {
            var clamped = Math.Clamp(value, Minimum, Maximum - LargeChange);
            if (_Value == clamped) return;
            _Value = clamped;
            Invalidate();
            var scrollEventArgs = new ScrollEventArgs(ScrollEventType.ThumbPosition, (int)_Value);
            Scroll?.Invoke(this, scrollEventArgs);
        }
    }

    private void HScrollBar_Draw(object? sender, EventArgs e)
    {
        Background.StartDrawing();
        Background.FillRectangle(0, 0, Width, Height, BackColor);
        Background.EndDrawing();

        float thumbWidth = Math.Max(Width * LargeChange / (Maximum - Minimum), 10);
        float trackWidth = Width - thumbWidth;
        float thumbX = (Value - Minimum) / (Maximum - LargeChange - Minimum) * trackWidth;

        Thumb.StartDrawing();
        Thumb.FillRectangle((int)thumbX, 0, (int)thumbWidth, Height, ForeColor);
        Thumb.EndDrawing();

        Border.StartDrawing();
        Border.DrawRectangle(0, 0, Width, Height, BorderColor, BorderSize);
        Border.EndDrawing();
    }
    private void HScrollBar_MouseDown(object? sender, MouseEvent e)
    {
        float x = e.X;
        float y = e.Y;
        if (IsPointInThumb(x, y))
        {
            isDragging = true;
            dragOffsetX = x - GetThumbX();
        }
    }
    private void HScrollBar_MouseMove(object? sender, MouseEvent e)
    {
        if (isDragging)
        {
            float x = e.X;
            float y = e.Y;
            float thumbWidth = Math.Max(Width * LargeChange / (Maximum - Minimum), 10);
            float trackWidth = Width - thumbWidth;
            float relativeX = x - Left - dragOffsetX;
            float ratio = Math.Clamp(relativeX / trackWidth, 0, 1);
            Value = Minimum + (Maximum - LargeChange - Minimum) * ratio;
        }
    }
    private void HScrollBar_MouseUp(object? sender, MouseEvent e)
    {
        isDragging = false;
    }
    private void HScrollBar_MouseLeave(object? sender, EventArgs e)
    {
        isDragging = false;
    }

    private bool IsPointInThumb(float x, float y)
    {
        float thumbWidth = Math.Max(Width * LargeChange / (Maximum - Minimum), 10);
        float thumbX = GetThumbX();
        return x >= thumbX && x <= thumbX + thumbWidth;
    }

    private float GetThumbX()
    {
        float thumbWidth = Math.Max(Width * LargeChange / (Maximum - Minimum), 10);
        float trackWidth = Width - thumbWidth;
        return (Value - Minimum) / (Maximum - LargeChange - Minimum) * trackWidth;
    }
}
