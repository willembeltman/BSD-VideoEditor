# 🔧 GitHub Project Description – BSD-VideoEditor
BSD-VideoEditor – A custom video editor written in C# using FFmpeg, SharpDX, and EntityFrameworkZip.

BSD-VideoEditor (BSD = Beltman Software Design) is a work-in-progress personal project born out of frustration with a bug in DaVinci Resolve. The goal: build a no-nonsense video editor that just works, using powerful low-level libraries like FFmpeg and DirectX.

## 🎥 Project Features & Stack

🧠 Built in C# with SharpDX for GPU rendering

🧰 Uses FFmpeg for decoding and playback

💾 Built around a custom database system stored in zip files via EntityFrameworkZip (also created during this project)

🖼️ Custom Direct2D and later Direct3D rendering to get live video previews on screen

🧱 Custom UI system (like Windows Forms, but GPU-accelerated) for high-performance rendering of timeline and preview controls

## 🚧 What currently works

Basic playback of a video clip in a timeline

GPU-accelerated preview window

Timeline editor UI with movable video clips

Playback works (as long as you don’t scroll too aggressively 😉)

## 💡 Why?

    “There’s a bug in DaVinci Resolve. I thought — how hard can it be?”
    — Me, just before building a whole rendering engine and embedded database format.