# Shit Compressor

![Screenshot](https://user-images.githubusercontent.com/43489288/102893687-f91dd180-449c-11eb-812e-d091c03bb956.png "Screenshot")

[Your shit is _apart_, and it's rather unbecoming of a web developer. It's supposed to be the opposite of that: _together_. Compressed in a small area. To achieve a solid level of shit-compression, squeeze your butt-cheeks together for 30 minutes.](https://discoelysium.gamepedia.com/Volumetric_Shit_Compressor)

## What is it?

Image compressor with graphical user interface. Does:

- PNG to JPG (lossy)
- JPG to JPG (lossy or lossless)
- PNG/JPG to WEBP (lossy, no alpha).

## Features

- Generates [SSIMULACRA](https://github.com/cloudinary/ssimulacra) and [Butteraugli](https://github.com/google/butteraugli) scores along with a SSIM Map and Edge Artifact map so that you know how shitty your compressed image is compared to the original.
- Contains 5 encoders (Guetzli, MozJpeg (lossy and lossless), JpegOptim and LibWebP) with the ability to preview and choose which output to save.
- Ability to enable encoders and adjust quality setting on a per-image level.
- Decoupled architecture let's you easily update encoders by replacing existing binaries without recompiling the entire program.
- Exposed internals let's you set configure all flags and arguments that each encoder offers.

## Usage

Drop and drop images > Optimize!

The preferred output will be saved in a folder called "optimized" located in the same folder as the input file.
If it's the first optimization run for that image, the smallest output of each format will be automatically designated the preferred output.
You can use the radio button to designate another output as preferred.

Successful outputs with the same MD5 hash are grouped together into a single entry.

Ssimulacra advises that:
If the Ssimulacra score is above 0.1 (or so), the distortion is likely to be perceptible / annoying.
If the Ssimulacra score is below 0.01 (or so), the distortion is likely to be imperceptible.

Butteraugli did not provide any advice on interpreting its score.

## Technical

Made with WPF with the .NET 5.0 framework.

Be warned this is my first WPF application, so expect bugs and inefficiencies!

## Requirements

x64 Windows 10 Version 1607 and above.

## Which Version to Download?

**Self-Contained** - Larger download (~41MB) and uncompressed size (~113MB), portable since everything you need are bundled together.

**Framework Dependent** - Smaller download (~11MB) and uncompressed size (~30MB), but requires the [.NET 5.0 Runtime](https://dotnet.microsoft.com/download/dotnet/current/runtime) to be installed on your computer to work.

## Source of Binaries Used

- **Guetzli** - version 1.0.1, built from source using Visual Studio 2019, modified from [original](https://github.com/google/guetzli/releases/tag/v1.0.1) to allow setting quality < 84.
- **MozJpeg** - version 4.0.0, built from [source](https://github.com/mozilla/mozjpeg/releases/tag/v4.0.0) using CMake and Visual Studio 2019.
- **JpegOptim** - version 1.4.6 built from [source](https://github.com/tjko/jpegoptim/releases/tag/RELEASE.1.4.6) using CMake and Visual Studio 2019.
- **LibWebP** - version 0.4.1, [binaries](https://storage.googleapis.com/downloads.webmproject.org/releases/webp/index.html) released by Google.
- **Ssimulacra** - version 30 Jun 2020, built from [source](https://gist.github.com/jialiang/c614b72d7b67ae93bcfe437f1b481a52) using Visual Studio 2019, modified from [original](https://github.com/cloudinary/ssimulacra) to accept a 3rd argument as SSIM and Edge Diff map prefix.
- **Butteraugli** - version 20 March 2019, built from [source](https://github.com/google/butteraugli) using Visual Studio 2019.

Dependencies of binaries downloaded and built using Vcpkg.

## Credits

The dependency libwebp-net-core is my own .NET 5.0 port of Imazen's [libwebp-net](https://github.com/imazen/libwebp-net).
