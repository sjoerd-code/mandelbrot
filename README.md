## What is this?:

This is a mandelbrot renderer i made in opengl using opentk and dear imgui.

## How to build:

Install .NET 6: https://dotnet.microsoft.com/en-us/download/dotnet/6.0

restore dependencies: ``dotnet restore``

run in debug mode: ``dotnet run``

build for linux: ``dotnet publish -o ./build/linux --sc True -r linux-x64``

build for windows: ``dotnet publish -o ./build/windows --sc True -r win-x64``

## Controls:
- ``wasd:`` move camera position
- ``arrows:`` change zoom level

## Screenshots:
<img src="https://user-images.githubusercontent.com/59654421/180652151-d774d6ff-b9a7-49ed-98ea-5ac7d38b9aad.png" width="600" />
<img src="https://user-images.githubusercontent.com/59654421/180652278-8beee263-2af8-40cf-8b43-8ace2ba0ce92.png" width="600" />
