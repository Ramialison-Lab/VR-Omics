# VR-Omics Documentation

VR-Omics is a software designed for the analysis and exploration of spatially resolved transcriptomics data. It provides powerful tools to visualize and interpret complex gene expression patterns in 3D desktop and virtual reality environments.

## Installation

There are two versions of VR-Omics available:

1. **Standalone Version**: You can download the standalone version of VR-Omics by clicking [here](https://bridges.monash.edu/articles/software/Spatially_Resolved_Transcriptomics_Exploration_in_3D_desktop_and_Virtual_Environments_with_VR-Omics_-_Application/20220312).

2. **GitHub Version**: The GitHub version allows you to run VR-Omics in Unity. To set up the GitHub version, follow these steps:

   - Download the VR-Omics code from the [GitHub repository](https://github.com/Ramialison-Lab/VR-Omics/).
   - Unzip the downloaded file.
   - Open Unity or Unity Hub.
   - Import the VR-Omics code into Unity.

   > Note: The GitHub version does not include the Python environments required for data processing.

## Python Environments

If you are using the GitHub version of VR-Omics, you need to download the Python environments for data processing. The Python environments can be found [here](https://bridges.monash.edu/articles/software/Spatially_Resolved_Transcriptomics_Exploration_in_3D_Desktop_and_Virtual_Reality_Environments_with_VR-Omics_-_Python_executables/22207903).

To set up the Python environments, follow these steps:

1. Save the downloaded Python environments in the following folder structure: `Assets/Scripts/Python_exe`.
2. Unzip the Python executable files and ensure they are named correctly:

   - For Visium (Download): `exe_Scanpy`
   - For Visium (Load): `exe_scanpy_upload`
   - For Xenium: `exe_xenium`
   - For MERFISH: `exe_merfish`
   - For Stomics: `exe_stomics`

## Test Data

We provide pre-processed test data that you can use with VR-Omics. You can download the test data [here](https://bridges.monash.edu/articles/dataset/Spatially_Resolved_Transcriptomics_Exploration_in_3D_Desktop_and_Virtual_Reality_Environments_with_VR-Omics_-_Test_data/22207579).

> **Disclaimer**: The test data provided is for demonstration purposes only and should not be used for scientific findings or publications. It is not intended to represent real-world data and should be used solely for testing and familiarizing yourself with the VR-Omics software.

## System Requirements

To ensure smooth operation of VR-Omics, please ensure that your system meets the following requirements:

- Operating System: Windows, macOS, or Linux
- Unity version 2021.3.11f1
- Processor: Intel Core i7 or equivalent
- RAM: 32 GB or higher

## System Configuration Used for Testing

During our testing, we used the following system configuration:

- Laptop: Alienware R15
- Processor: Intel(R) Core(TM) i7-10870H CPU @ 2.20GHz   2.21 GHz
- RAM: 32 GB

For more detailed information and guidance on using VR-Omics, refer to the [full VR-Omics documentation](https://ramialison-lab.github.io/pages/vromics.html).


## License

MIT License

Copyright (c) 2025 Denis Bienroth [Bienroth, Murdoch Children's Research Institute]

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
