# Shit Compressor

![Screenshot](https://user-images.githubusercontent.com/43489288/103145948-d03b5c00-477c-11eb-814c-95611c877a92.jpg "Screenshot")

[Your shit is _apart_, and it's rather unbecoming of a web developer. It's supposed to be the opposite of that: _together_. Compressed in a small area.](https://discoelysium.gamepedia.com/Volumetric_Shit_Compressor)

## What is it?

Image compressor with graphical user interface. Does:

- PNG to JPG (lossy)
- JPG to JPG (lossy or lossless)
- PNG/JPG to WEBP (lossy).
- PNG/JPG to AVIF (lossy)

## Features

- Generates [SSIMULACRA](https://github.com/cloudinary/ssimulacra) and [Butteraugli](https://github.com/google/butteraugli) scores along with a SSIM Map and Edge Artifact map so that you know how shitty your compressed image is compared to the original (Butteraugli only works for png and jpg output).
- Contains 6 encoders (Guetzli, MozJpeg (lossy and lossless), JpegOptim, LibWebP and LibAvif) with the ability to preview and choose which output to save.
- Ability to enable encoders and adjust quality setting on a per-image level.
- Decoupled architecture let's you easily update encoders by replacing existing binaries without recompiling the entire program.
- Exposed internals let's you set configure all flags and arguments that each encoder offers.

## Usage

**Drop and drop images > Optimize!**

- The preferred output will be saved in a folder called "optimized" located in the same folder as the input file.

- If it's the first optimization run for that image, the smallest output of each format will be automatically designated the preferred output.
- You can use the radio button to designate another output as preferred.
- Successful outputs with the same MD5 hash are grouped together into a single entry.
- Guetzli and Butteraugli are disabled by default because **THEY ARE SLOW** and not necessarily any better. Enable them in the settings if you want.

**Ssimulacra advises that:**

If the Ssimulacra score is above 0.1 (or so), the distortion is likely to be perceptible / annoying.
If the Ssimulacra score is below 0.01 (or so), the distortion is likely to be imperceptible.

**Butteraugli did not provide any advice on interpreting its score.**

## Technical

Made with WPF with the .NET 8.0 framework.

## Requirements

x64 Windows 10 Version 1607 and above.

## Which Version to Download?

### For Release 1.1.4 onwards:

Only 1 self-contained x64-Windows binary is provided.

### For Release 1.1.3 and below:

**Self-Contained** - Larger download (~67MB) and uncompressed size (~173MB), portable since everything you need are bundled together.

**Framework Dependent** - Smaller download (~13MB) and uncompressed size (~36MB), but requires the [.NET 5.0 Runtime](https://dotnet.microsoft.com/download/dotnet/current/runtime) to be installed on your computer to work.

## Source of Binaries Used

- **Guetzli** - version 1.0.1, built from source using Visual Studio 2022, modified from [original](https://github.com/google/guetzli/releases/tag/v1.0.1) to allow setting quality < 84.
- **MozJpeg** - version 4.1.1, built from [source](https://github.com/mozilla/mozjpeg/releases/tag/v4.1.1) using CMake and Visual Studio 2022.
- **JpegOptim** - version 1.5.5, [binaries](https://github.com/tjko/jpegoptim/releases/tag/v1.5.5) released by Timo Kokkonen.
- **LibWebP** - version 1.3.2, [binaries](https://storage.googleapis.com/downloads.webmproject.org/releases/webp/index.html) released by Google.
- **SSIM-X** - version v1.0.2, built from [source](https://github.com/jialiang/SSIM-X) using Visual Studio 2022.
- **Butteraugli** - version 20 March 2019, built from [source](https://github.com/google/butteraugli) using Visual Studio 2019.
- **JpegTran** - version 12 Januarary 2020, [binaries](https://jpegclub.org/jpegtran/) released by the Independent JPEG Group.
- **LibAvif** - version 1.0.4 (using AOM 3.8.1), built from [source](https://github.com/AOMediaCodec/libavif/releases/tag/v1.0.4) using CMake and Visual Studio 2022.

Dependencies of binaries downloaded and built using Vcpkg.

## Known Issues

- Fails with input filepaths containing non-ASCII characters. All the binaries used were built with C++ which doesn't natively handle arguments containing non-ASCII characters.
- Generally only 8 bit per pixel RGB or RGBA PNG or 8 bit per pixel JPG is fully supported. Indexed colors work fine too. Also, I think it's best to make sure your input is sRGB color profile and 2.2 gamma.

## Note

I'm not affiliated with Zaum, I just got the inspiration to create this after playing Disco Elysium.
