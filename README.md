# Diablo 4 Optical Character Recognition

Companion app for going through endless loot for those good item rolls.

D4Ocr app reads screen and highlights wanted item stats to make it easier to sort out loot.
D4Ocr periodically scans your Diablo 4 game screen and renders custom overlay.

**_NOTE:_** IT DOES NOT MODIFY DIABLO 4 GAME IN ANY WAY!!!

## TODO:
* Write usage instructions
* Github release builds
* Highlight bad nightmare dungeon modifiers
* Optimize scan speed and tesseract configs.

Optional TODO:
* Stat ranges?
* Turn on and off by key press? -> not really possible because requires focus into  console window.

## Features

### OCR

Project uses Tesseract open source wrapper for OCR.

https://github.com/charlesw/tesseract

And training models from:

https://github.com/tesseract-ocr/tessdata_fast

### Capturing game screen

Capturing desktop/game screen is done with native Windows interop and with legacy PInvoke library.
There's new CsWin32 library which uses code generation for interop calls

https://github.com/dotnet/pinvoke
https://github.com/microsoft/CsWin32
https://www.cyotek.com/blog/capturing-screenshots-using-csharp-and-p-invoke

### Overlay

Overlay is drawn by GameOverlay.Net library.

https://github.com/michel-pi/GameOverlay.Net

## Known problems / solutions

### Problem: Capture size is smaller than rendered size

Diablo 4 runs on borderless windowed resolution and uses dynamic resolution scaling.
So actual rendered resolution can be different than screen captured resolution, which means that there is need for scaling captured pixel coordinates into game resolution coordinates.

This is solved with configurable game resolution setting in app.config.

### Problem: Rectangle highlights caused next OCR captures to fail

Use non obstructive highlights which dont affect next OCR captures.

### Bug: Screen capture is smaller than rendered full screen.

There seems to be some bug on screen capture code which causes this. Check debugged image captures.