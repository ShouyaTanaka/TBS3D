using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Utility;

public class MapGenerator : MonoBehaviour
{
    int mapWidth;
    int mapHeight;
    List<GameObject> cubes = new List<GameObject>();

    void Awake()
    {
        FindObjectOfType<Log>().AddLog("ロード開始");
    }

    async void Start()
    {
        // CSVデータ読み込み
        int[,] mapData = CSVLoader.CSVMapLoad("map00");

        // マップサイズの取得
        mapWidth = mapData.GetLength(0);
        mapHeight = mapData.GetLength(1);
        FindObjectOfType<Log>().AddLog("横：" + mapWidth);
        FindObjectOfType<Log>().AddLog("縦：" + mapHeight);

        // マップ生成処理
        await GenerateMapAsync(mapData);
    }

    /// <summary>
    /// マップ生成のメイン処理
    /// </summary>
    /// <param name="mapData">CSVから読み取ったデータ</param>
    /// <returns></returns>
    public async UniTask GenerateMapAsync(int[,] mapData)
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int z = 0; z < mapHeight; z++)
            {
                int type = mapData[x, z];
                int y = (Map)type == Map.Blocked ? 1 : 0;

                // エフェクト
                _ = SetCubeEffectAsync(x, y, z ,type);
                if (y == 1)
                {
                    _ = SetCubeEffectAsync(x, y - 1, z ,type);
                }

                await UniTask.Delay(20);
            }
        }

        // 全体の演出待機
        await UniTask.Delay(1000);

        // メッシュ結合
        CombineMeshesForCollider();

        // ステージ点滅演出
        await FlashStageEffectAsync();

        FindObjectOfType<Log>().AddLog("ステージ完成！");
    }

    /// <summary>
    /// 生成完了時の処理
    /// </summary>
    private async UniTask FlashStageEffectAsync()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer rend in renderers)
        {
            if (rend.material.HasProperty("_EmissionColor"))
            {
                rend.material.EnableKeyword("_EMISSION");
                rend.material.SetColor("_EmissionColor", Color.yellow);
            }
        }

        await UniTask.Delay(300);

        foreach (Renderer rend in renderers)
        {
            if (rend.material.HasProperty("_EmissionColor"))
            {
                rend.material.SetColor("_EmissionColor", Color.black);
            }
        }
    }

    /// <summary>
    /// 配置処理
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    private async UniTask SetCubeEffectAsync(float x, float y, float z, int type)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.parent = this.transform;
        cube.transform.localPosition = new Vector3(x, y, z);
        cube.transform.localScale = Vector3.zero;
        // データ付与
        cube.AddComponent<GridData>();
        cube.GetComponent<GridData>().gridPos = new Vector2Int((int)x, (int)z);
        cube.GetComponent<GridData>().gridState = (Map)type;
        // 配列格納（結合用）
        cubes.Add(cube);

        // 拡大演出（0.3秒）
        float duration = 0.3f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float scale = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            cube.transform.localScale = new Vector3(scale, scale, scale);
            await UniTask.Yield();
        }

        // 光らせる（0.1秒）
        Renderer rend = cube.GetComponent<Renderer>();
        if (rend.material.HasProperty("_EmissionColor"))
        {
            rend.material.EnableKeyword("_EMISSION");
            rend.material.SetColor("_EmissionColor", Color.yellow);
            await UniTask.Delay(100);
            rend.material.SetColor("_EmissionColor", Color.black);
        }

        FindObjectOfType<Log>().AddLog($"設置完了 {x}, {y}, {z}");
    }

    /// <summary>
    /// 当たり判定を親オブジェクトへ結合
    /// </summary>
    private void CombineMeshesForCollider()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        List<CombineInstance> combine = new List<CombineInstance>();

        foreach (MeshFilter mf in meshFilters)
        {
            if (mf.gameObject == this.gameObject) continue;

            CombineInstance ci = new CombineInstance();
            ci.mesh = mf.sharedMesh;
            ci.transform = mf.transform.localToWorldMatrix * transform.worldToLocalMatrix;
            combine.Add(ci);
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        combinedMesh.CombineMeshes(combine.ToArray(), true, true);

        MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = combinedMesh;

        foreach (GameObject cube in cubes)
        {
            Destroy(cube.GetComponent<Collider>());
        }
    }
}
