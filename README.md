# DotImageDemoImproved

This is a copy of the Atalaosoft DotImage Demo available from [www.atalasoft.com](https://www.atalasoft.com).

## Improvements Over Original
- This one is compiled x64 (which actualy, Atalasoft started to do by default for non-TWAIN demos staring in 11.5)
- Long time annoyance: the Position / Selection indicator was always too small when selecting so yo ucould not see the actual full values
- This version alters the default PdfDecoder
- It's defaulted to 300 DPI instead of 200 DPI
- In the Atalasoft demo, Pdf native (embedded) annotaions were set to RenderNone. In this we set to RenderAll. This is a minor change but is more practical for out of hte box use
- View Changes
	- Default is now Fit to Width
	- Added a new View menu to let you choose various auto zoom or manual zoom levels


## Feature Requests / Ideas
- Quadrilateral Warp - The default raw properties are ? OK ? if you want to explore the raw command, but really hard to get right. Considering a polygon select or other visual tool to let you rewarp the image. NOTE: may need to be 100% view as does weird stuff when in autozom mode

## How to use
This demo source code has been available free to use and modify from Atalasoft for years, even thouth their SDK is closed/prorprietary. This means that this demo too is avilable under a permissive license (MIT License), but there are components that require an Atalasoft DotImge Document Imaging license to use.

Assuming you have a license for Atalasoft.DotImage Document Imging and PdfReader adons

1. Install the latest Atalasoft DotImage (this was based on 11.5 , current version as of June 2025 is 11.5.0.7)
2. Activate your licenses for DotImage or request an evaluation (good for 30 days, may watermark otput images)
3. Ensure that the references to the Atalasoft components within are resolving to the ones in your sdk - default location is `C:\Program Files (x86)\Atalasoft\DotImage 11.5\bin\4.6.2\x64`
	- NOTE: if you keep your Atalasoft install elsehwere edit the References section of the project properties to reflect that
4. Build and run
5. If you get an error about Unable to find Atalasoft.\*.lic in the following locations... that means you're missing the needed licensing

## Last Update
Updated Saturday, June 7, 2025 - DigitalSorceress

