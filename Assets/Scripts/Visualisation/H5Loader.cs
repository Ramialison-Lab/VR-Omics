/*
* Copyright (c) 2023 Murdoch Children's Research Institute, Parkville, Melbourne
* author: Denis Bienroth
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"),
* to deal in the Software without restriction, including without limitation
* the rights to use, copy, modify, merge, publish, distribute, sublicense,
* and/or sell copies of the Software, and to permit persons to whom the Software
* is furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in all
* copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
* INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
* PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
* HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
* CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE
* OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using HDF.PInvoke;
using JetBrains.Annotations;
using UnityEngine;

namespace UnityH5Loader {
    public static class H5Loader {

        static readonly ulong[] MaxDimensions = {10000, 10000, 10000};

        [PublicAPI]
        public static int[] LoadIntDataset(string filePath, string datasetName) {
            long[] longArray = LoadDataset<long>(filePath, datasetName);
            int[] integerArray = longArray.Select(item => (int) item).ToArray();
            return integerArray;
        }

        [PublicAPI]
        public static float[] LoadFloatDataset(string filePath, string datasetName) {
            double[] doubleArray = LoadDataset<double>(filePath, datasetName);
            float[] floatArray = doubleArray.Select(item => (float) item).ToArray();
            return floatArray;
        }


        [PublicAPI]
        public static string[] LoadStringDataset(string filePath, string dataSetName) {
            //With much Help from:  https://stackoverflow.com/questions/23295545/reading-string-array-from-a-hdf5-dataset
            long fileId = H5F.open(filePath, H5F.ACC_RDONLY);
            
            string longJoinedString;
            int stringLength;
            try {
                
                long datasetId = H5D.open(fileId, dataSetName);
                long spaceID = H5D.get_space(datasetId);
                long dataType = H5D.get_type(datasetId);

                int[] dimensions = GetDatasetDimensions(spaceID);

                stringLength = (int) H5T.get_size(dataType);
                byte[] buffer = new byte[dimensions[0] * stringLength];

                GCHandle gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);

                
                try {
                    H5D.read(datasetId, dataType, H5S.ALL, H5S.ALL, H5P.DEFAULT, gch.AddrOfPinnedObject());
                    longJoinedString = Encoding.ASCII.GetString(buffer);
                }
                finally {
                    gch.Free();
                    H5D.close(dataType);
                    H5D.close(spaceID);
                    H5D.close(datasetId);
                }
            }
            finally {
                H5F.close(fileId);
            }

            return longJoinedString.SplitInParts(stringLength).Select(ss => (string) (object) ss).ToArray();
        }
        
        static IEnumerable<string> SplitInParts(this string theLongString, int partLength) {
            if (theLongString == null)
                throw new ArgumentNullException(nameof(theLongString));
            if (partLength <= 0)
                throw new ArgumentException("Part length has to be positive.", nameof(partLength));

            for (var i = 0; i < theLongString.Length; i += partLength)
                yield return theLongString.Substring(i, Math.Min(partLength, theLongString.Length - i));
        }

        [PublicAPI]
        public static int[,] Load2dIntDataset(string filePath, string datasetName) {
            long[,] doubleArray = Load2DDataset<long>(filePath, datasetName);
            int[,] intArray = new int[doubleArray.GetLength(0),doubleArray.GetLength(1)];
            for (int i = 0; i < doubleArray.GetLength(0); i++) {
                for (int j = 0; j < doubleArray.GetLength(1); j++) {
                    intArray[i, j] = (int)doubleArray[i, j];
                }
            }
            return intArray;
        }
        
        public static float[,] Load2dFloatDataset(string filePath, string datasetName) {
            double[,] doubleArray = Load2DDataset<double>(filePath, datasetName);
            float[,] floatArray = new float[doubleArray.GetLength(0),doubleArray.GetLength(1)];
            for (int i = 0; i < doubleArray.GetLength(0); i++) {
                for (int j = 0; j < doubleArray.GetLength(1); j++) {
                    floatArray[i, j] = (float)doubleArray[i, j];
                }
            }
            return floatArray;
        }

        [PublicAPI]
        public static T[] LoadDataset<T>(string filePath, string datasetName) {
            
            if (!File.Exists(filePath)) throw new FileNotFoundException($"Loading dataset {datasetName} from file that doesn't exist {filePath}");
            long fileId = H5F.open(filePath, H5F.ACC_RDONLY);
            
            T[] resultArray;

            try {

                long datasetId = H5D.open(fileId, datasetName);
                if (datasetId == -1) throw new ArgumentException($"Dataset could not be opened. Check filepath exists and is correct. FilePath = {filePath}");
                long typeId = H5D.get_type(datasetId);
                long spaceID = H5D.get_space(datasetId);

                int[] dimensions = GetDatasetDimensions(spaceID);

                resultArray = new T[dimensions[0]];
                GCHandle gch = GCHandle.Alloc(resultArray, GCHandleType.Pinned);

                try {
                    H5D.read(datasetId, typeId, H5S.ALL, H5S.ALL, H5P.DEFAULT, gch.AddrOfPinnedObject());
                }
                finally {
                    gch.Free();
                    H5D.close(typeId);
                    H5D.close(spaceID);
                    H5D.close(datasetId);
                }
            }
            finally {
                H5F.close(fileId);
            }

            return resultArray;

        }

        static T[,] Load2DDataset<T>(string filePath, string datasetName)
        {

            if (!File.Exists(filePath)) throw new FileNotFoundException($"Loading dataset {datasetName} from file that doesn't exist {filePath}");
            long fileId = H5F.open(filePath, H5F.ACC_RDONLY);

            T[,] resultArray = new T[2, 2];
            try
            {

                ulong[] start = { 0, 0 };
                ulong[] count = { 0, 0 };

                long datasetId = H5D.open(fileId, datasetName);
                var datatype = H5D.get_type(datasetId);
                var spaceId = H5D.get_space(datasetId);
                int rank = H5S.get_simple_extent_ndims(spaceId);
                ulong[] maxDims = new ulong[rank];
                ulong[] dims = new ulong[rank];
                H5S.get_simple_extent_dims(spaceId, dims, maxDims);

                count[0] = dims[0];
                count[1] = dims[1];

                // Define file hyperslab. 
                long status = H5S.select_hyperslab(spaceId, H5S.seloper_t.SET, start, null, count, null);

                // Define the memory dataspace.
                resultArray = new T[dims[0], dims[1]];
                var memId = H5S.create_simple(rank, dims, null);

                // Define memory hyperslab. 
                status = H5S.select_hyperslab(memId, H5S.seloper_t.SET, start, null,
                    count, null);

                // Read data from hyperslab in the file into the hyperslab in 
                // memory and display.             
                GCHandle handle = GCHandle.Alloc(resultArray, GCHandleType.Pinned);

                try
                {
                    H5D.read(datasetId, datatype, memId, spaceId,
                        H5P.DEFAULT, handle.AddrOfPinnedObject());
                }
                finally
                {
                    handle.Free();
                    H5S.close(status);
                    H5S.close(memId);
                    H5S.close(spaceId);
                    H5D.close(datatype);
                    H5D.close(datasetId);
                }
            }
            finally
            {
                H5F.close(fileId);
            }
            return resultArray;
        }

        static int[] GetDatasetDimensions(long spaceID) {
            int numberOfDimensions = H5S.get_simple_extent_ndims(spaceID);

            int[] dimensions = new int[0];
            if (numberOfDimensions >= 0) {
                ulong[] dims = new ulong[numberOfDimensions];
                H5S.get_simple_extent_dims(spaceID, dims, MaxDimensions);
                dimensions = ConvertDimensionsToIntegers(dims);
            }

            return dimensions;
        }

        static int[] ConvertDimensionsToIntegers(ulong[] dims) {
            if (dims == null) throw new ArgumentNullException(nameof(dims));
            int[] dimensions = new int[dims.Length];
            for (int i = 0; i < dims.Length; i++) {
                dimensions[i] = (int) dims[i];
            }

            return dimensions;
        }
    }

}
