using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Utility;

public class MapEditorWindow : EditorWindow
{
    private int width = 18;
    private int height = 10;
    private int newWidth;
    private int newHeight;
    private int[,] mapData;

    private Map currentType = Map.Ground;

    private Vector2 scrollPos;

    [MenuItem("Tools/Map CSV Editor")]
    public static void ShowWindow()
    {
        GetWindow<MapEditorWindow>("Map CSV Editor");
    }

    private void OnEnable()
    {
        newWidth = width;
        newHeight = height;
        InitMap();
    }

    private void InitMap()
    {
        mapData = new int[width, height];
    }

    private void ResizeMap(int newWidth, int newHeight)
    {
        int[,] newMap = new int[newWidth, newHeight];

        for (int x = 0; x < Mathf.Min(width, newWidth); x++)
        {
            for (int y = 0; y < Mathf.Min(height, newHeight); y++)
            {
                newMap[x, y] = mapData[x, y];
            }
        }

        mapData = newMap;
        width = newWidth;
        height = newHeight;
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("マップサイズ変更");

        newWidth = EditorGUILayout.IntField("幅", newWidth);
        newHeight = EditorGUILayout.IntField("高さ", newHeight);

        if (newWidth != width || newHeight != height)
        {
            if (GUILayout.Button("マップリサイズ"))
            {
                ResizeMap(newWidth, newHeight);
            }
        }

        if (GUILayout.Button("マップ初期化"))
        {
            InitMap();
        }

        currentType = (Map)EditorGUILayout.EnumPopup("選択中のタイルタイプ", currentType);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        for (int y = height - 1; y >= 0; y--)
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < width; x++)
            {
                Color originalColor = GUI.backgroundColor;
                GUI.backgroundColor = GetColor((Map)mapData[x, y]);

                if (GUILayout.Button(mapData[x, y].ToString(), GUILayout.Width(30), GUILayout.Height(30)))
                {
                    mapData[x, y] = (int)currentType;
                }

                GUI.backgroundColor = originalColor;
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space();

        if (GUILayout.Button("CSVとして保存"))
        {
            SaveCSV();
        }

        if (GUILayout.Button("CSVを読み込む"))
        {
            LoadCSV();
        }
    }

    private void SaveCSV()
    {
        string path = EditorUtility.SaveFilePanel("CSVとして保存", "Assets/Resources/Maps", "map00.csv", "csv");
        if (string.IsNullOrEmpty(path)) return;

        using (StreamWriter writer = new StreamWriter(path))
        {
            for (int y = height - 1; y >= 0; y--)
            {
                List<string> line = new List<string>();
                for (int x = 0; x < width; x++)
                {
                    line.Add(mapData[x, y].ToString());
                }
                writer.WriteLine(string.Join(",", line));
            }
        }

        AssetDatabase.Refresh();
        Debug.Log("CSV保存完了: " + path);
    }

    private void LoadCSV()
    {
        string path = EditorUtility.OpenFilePanel("CSVを読み込む", "Assets/Resources/Maps", "csv");
        if (string.IsNullOrEmpty(path)) return;

        string[] lines = File.ReadAllLines(path);
        height = lines.Length;
        width = lines[0].Split(',').Length;

        mapData = new int[width, height];

        for (int y = 0; y < height; y++)
        {
            string[] line = lines[y].Trim().Split(',');

            for (int x = 0; x < width; x++)
            {
                if (int.TryParse(line[x], out int value))
                {
                    mapData[x, height - y - 1] = value;
                }
            }
        }

        Debug.Log("CSV読み込み完了: " + path);
    }

    private Color GetColor(Map type)
    {
        switch (type)
        {
            case Map.Blocked: return Color.gray;
            case Map.Ground: return Color.green;
            case Map.PlayerSpawn1: return Color.cyan;
            case Map.PlayerSpawn2: return Color.cyan;
            case Map.PlayerSpawn3: return Color.cyan;
            case Map.PlayerSpawn4: return Color.cyan;
            case Map.EnemySpawn1: return Color.red;
            case Map.EnemySpawn2: return Color.red;
            case Map.EnemySpawn3: return Color.red;
            case Map.EnemySpawn4: return Color.red;
            default: return Color.white;
        }
    }
}
