<div align="center">
  <img src="logo.svg" alt="EmojiTray Logo" width="96" />
  <h1>EmojiTray</h1>
  <p>An ultra-lightweight, native dark-mode emoji picker that lives in your Windows system tray.</p>

  <p>
    <img src="https://img.shields.io/badge/Platform-Windows%2010%20%7C%2011-0078D6?style=for-the-badge&logo=windows&logoColor=white" alt="Platform Windows" />
    <img src="https://img.shields.io/badge/Language-C%23%20(.NET%204.8)-239120?style=for-the-badge&logo=csharp&logoColor=white" alt="Language C#" />
    <img src="https://img.shields.io/badge/Size-~20%20KB-60CDFF?style=for-the-badge" alt="Size ~20 KB" />
    <img src="https://img.shields.io/badge/License-MIT-4ADE80?style=for-the-badge" alt="License MIT" />
  </p>
</div>

---

## Features

- **Lives in the System Tray** — Sits quietly inside your taskbar overflow arrow (`^`) until clicked.
- **Top 5 Recently Used** — Automatically tracks and persists your 5 most recent emojis across reboots.
- **Instant Keyword Search** — Auto-focuses the search bar on open so you can type immediately (`cry`, `skull`, `fire`, `check`, etc.).
- **One-Click Clipboard Copy** — Clicking any emoji copies it straight to your clipboard and closes the popup.
- **Smooth Dark-Mode UI** — Native Windows 11 styling, full-color `Segoe UI Emoji` rendering, and 60fps smooth wheel scrolling.
- **Built-in Startup Toggle** — Right-click the tray icon and check **Start with Windows** to launch automatically on boot.
- **Zero Dependencies** — Compiles directly with the `.NET Framework` compiler already built into Windows.

---

## Quick Start

1. Download **`EmojiTray.exe`** from the [Releases](https://github.com/MixGaming25/Emoji-Tray/releases) page.
2. Place it anywhere permanent (e.g., `C:\Tools\EmojiTray.exe`) and double-click to run.
3. Click the **`^`** arrow in your Windows system tray and left-click the smiley icon to open the picker.
4. *(Optional)* Right-click the tray icon and enable **Start with Windows**.

> **Note:** Because `EmojiTray.exe` is an unsigned indie binary, Windows SmartScreen may show *"Windows protected your PC"* on first launch. Click **More info → Run anyway**, or compile it yourself in 1 second using `build.cmd` below.

---

## Build from Source

No Visual Studio or external SDKs required—Windows includes the C# compiler (`csc.exe`) out of the box:

```bat
git clone https://github.com/MixGaming25/Emoji-Tray.git
cd Emoji-Tray
build.cmd
```

This compiles `EmojiTray.cs` into a standalone `EmojiTray.exe` (~20 KB).

---

## Files

| File | Description |
| :--- | :--- |
| `EmojiTray.cs` | Complete single-file C# source code (system tray host, registry startup toggle, and UI). |
| `build.cmd` | One-click build script using Windows' built-in `.NET Framework` C# compiler. |
| `logo.svg` | Vector project icon. |

---

## Data Storage

Your 5 recently used emojis are saved locally in:

```text
%APPDATA%\TrayEmojiPicker\recent.json
```
