@echo off
set CSC="%SystemRoot%\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
if not exist %CSC% set CSC="%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\csc.exe"

%CSC% /nologo /target:winexe /optimize+ /out:EmojiTray.exe /reference:System.Windows.Forms.dll /reference:System.Drawing.dll /reference:System.Web.Extensions.dll EmojiTray.cs

if %errorlevel% equ 0 (
    echo Built EmojiTray.exe successfully!
) else (
    echo Build failed.
    pause
)