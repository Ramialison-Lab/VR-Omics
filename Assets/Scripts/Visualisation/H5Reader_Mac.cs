using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;


public class H5Reader_Mac : MonoBehaviour
{
    public List<string> ReadH5Values(string filePath, string h5Path)
    {
        Debug.Log(filePath);
        Debug.Log(h5Path);
        List<string> values = new List<string>();

        using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            using (BinaryReader binaryReader = new BinaryReader(fileStream))
            {
                Debug.Log(FindH5Path(binaryReader, h5Path));

                // Find the specified path within the HDF5 file
                if (FindH5Path(binaryReader, h5Path))
                {
                    // Read the values at the specified path
                    values = ReadValues(binaryReader);
                }
            }
        }
        return values;
    }

    private bool FindH5Path(BinaryReader binaryReader, string h5Path)
    {
        // Go to the root group
        binaryReader.BaseStream.Seek(0, SeekOrigin.Begin);

        while (true)
        {
            byte[] headerBytes = binaryReader.ReadBytes(4);

            // Check if the header is valid
            if (Encoding.ASCII.GetString(headerBytes) != "OHDR")
            {
                break;
            }

            // Read the size of the header
            int headerSize = binaryReader.ReadInt32();

            // Read the header data
            byte[] headerData = binaryReader.ReadBytes(headerSize);

            // Extract the path from the header data
            string path = ExtractH5PathFromHeader(headerData);

            // Check if the current path matches the desired path
            if (path == h5Path)
            {
                return true;
            }
        }

        return false;
    }

    private string ExtractH5PathFromHeader(byte[] headerData)
    {
        int pathLength = BitConverter.ToInt16(headerData, 8);
        byte[] pathBytes = new byte[pathLength];
        Array.Copy(headerData, 10, pathBytes, 0, pathLength);
        string path = Encoding.ASCII.GetString(pathBytes);

        return path;
    }

    private List<string> ReadValues(BinaryReader binaryReader)
    {
        List<string> values = new List<string>();

        int dataSize = binaryReader.ReadInt32();
        int dataTypeSize = binaryReader.ReadInt16();
        binaryReader.ReadInt16(); // Skip the padding bytes

        if (dataTypeSize != 1)
        {
            throw new InvalidDataException("Unsupported data type size.");
        }

        int numValues = dataSize / dataTypeSize;

        for (int i = 0; i < numValues; i++)
        {
            byte valueSize = binaryReader.ReadByte();
            byte[] valueBytes = binaryReader.ReadBytes(valueSize);
            string value = Encoding.ASCII.GetString(valueBytes);
            values.Add(value);
        }

        return values;
    }
}

