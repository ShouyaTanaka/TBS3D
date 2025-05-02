using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CSVLoader
{
    public static int[,] CSVMapLoad(string fileName)
    {
        TextAsset csvFile = Resources.Load<TextAsset>($"Maps/{fileName}");
        if (csvFile == null)
        {
            Debug.LogError($"CSVファイル {fileName} が見つかりません！");
            return null;
        }

        string[] lines = csvFile.text.Trim().Split('\n');
        int height = lines.Length;
        int width = lines[0].Split(',').Length;

        int[,] mapData = new int[width, height];

        for (int y = 0; y < height; y++)
        {
            string[] lineData = lines[y].Trim().Split(',');

            for (int x = 0; x < width; x++)
            {
                int value = 0;

                if(lineData.Length > x && !string.IsNullOrWhiteSpace(lineData[x]))
                {
                    int.TryParse(lineData[x], out value);
                }

                mapData[x, height - y - 1] = value;
            }
        }
        return mapData;
    }
}