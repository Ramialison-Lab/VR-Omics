using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CSVHeaderInformation
{

    /// <summary>
    /// Return the index of the target in the header of a CSV file
    /// </summary>
    /// <param name="header">The header of a CSV file as unprocessed string</param>
    /// <param name="target">The name of the header to look for.</param>
    /// <returns>The Index of the position of the target in the header string</returns>
    public static int ReadCSVHeaderPosition(string header, string target)
    {
        int target_position = -1;

        var values = header.Split(',');
        target_position = Array.IndexOf(values, target);

        return target_position;
    }

}
