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

    /// <summary>
    /// Checks if the header starts with the string provided
    /// </summary>
    /// <param name="lines"></param>
    /// <param name="header"></param>
    /// <returns></returns>
    public static bool CheckForHeader_with_header(string line1, string header)
    {
        bool hasHeader = false;
        string[] values = line1.Split(',');
        if (values[0].Contains(header))
        {
            hasHeader = true;
        }

        return hasHeader;
    }

    /// <summary>
    /// Checks if the header row contains different datatypes (only strings) compared to the second row. 
    /// THIS MIGHT LEAD TO ERROR IF CSV OF STRINGS EXPECTED!
    /// </summary>
    /// <param name="lines"></param>
    /// <returns></returns>
    public static bool CheckForHeaderInCSV_without_header(string line1, string line2)
    {
        bool hasHeader = false;

        string[] values1 = line1.Split(',');
        bool string1_is_string = true;
        bool string2_is_string = true;
        string[] values2 = line2.Split(',');

        for (int i = 0; i < values1.Length; i++)
        {
            int value;
            float f_value;
            if (int.TryParse(values1[i], out value) || float.TryParse(values1[i], out f_value))
            {
                string1_is_string = false;
            }
        }

        for (int i = 0; i < values2.Length; i++)
        {
            int value;
            float f_value;
            if (int.TryParse(values2[i], out value) || float.TryParse(values2[i], out f_value))
            {
                string2_is_string = false;
            }
        }

        if (string1_is_string && !string2_is_string) hasHeader = true;


        return hasHeader;
    }
}
