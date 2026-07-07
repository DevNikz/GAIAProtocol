using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TerrainGrassManager : MonoBehaviour
{
    public static TerrainGrassManager Instance { get; private set; }

    [SerializeField]
    private Terrain terrain;

    [SerializeField]
    private float cellSize = 2f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public Terrain GetTerrain()
    {
        return terrain;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.buildIndex)
        {
            case 1:
                terrain = GameObject.FindAnyObjectByType<Terrain>();
                break;
            default:
                terrain = null;
                break;
        }
    }

    public class GrassSnapshot
    {
        public int xBase,
            yBase,
            width,
            height;
        public int[,] originalNormalLayer;
        public int[,] originalCorruptedLayer;
    }

    public GrassSnapshot CorruptTile(
        Vector3 worldCenter,
        int normalLayerIndex,
        int corruptedLayerIndex,
        int corruptedDensity = 16
    )
    {
        if (terrain == null)
        {
            Debug.LogError("TerrainGrassManager: Terrain reference not assigned.");
            return null;
        }

        TerrainData data = terrain.terrainData;
        GetDetailRect(
            worldCenter,
            data,
            out int xBase,
            out int yBase,
            out int width,
            out int height
        );

        GrassSnapshot snapshot = new GrassSnapshot
        {
            xBase = xBase,
            yBase = yBase,
            width = width,
            height = height,
            originalNormalLayer = data.GetDetailLayer(
                xBase,
                yBase,
                width,
                height,
                normalLayerIndex
            ),
            originalCorruptedLayer = data.GetDetailLayer(
                xBase,
                yBase,
                width,
                height,
                corruptedLayerIndex
            ),
        };

        //Debug.Log(data.GetDetailLayer(xBase, yBase, width, height, corruptedLayerIndex));

        int[,] clearedNormal = new int[height, width];
        int[,] filledCorrupted = new int[height, width];
        for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
            filledCorrupted[y, x] = corruptedDensity;

        data.SetDetailLayer(xBase, yBase, normalLayerIndex, clearedNormal);
        data.SetDetailLayer(xBase, yBase, corruptedLayerIndex, filledCorrupted);

        return snapshot;
    }

    public void RevertTile(GrassSnapshot snapshot, int normalLayerIndex, int corruptedLayerIndex)
    {
        if (snapshot == null || terrain == null)
            return;

        TerrainData data = terrain.terrainData;
        data.SetDetailLayer(
            snapshot.xBase,
            snapshot.yBase,
            normalLayerIndex,
            snapshot.originalNormalLayer
        );
        data.SetDetailLayer(
            snapshot.xBase,
            snapshot.yBase,
            corruptedLayerIndex,
            snapshot.originalCorruptedLayer
        );
    }

    private void GetDetailRect(
        Vector3 worldCenter,
        TerrainData data,
        out int xBase,
        out int yBase,
        out int width,
        out int height
    )
    {
        Vector3 terrainPos = terrain.transform.position;
        Vector3 size = data.size;

        float halfCell = cellSize * 0.5f;
        float minX = worldCenter.x - halfCell;
        float maxX = worldCenter.x + halfCell;
        float minZ = worldCenter.z - halfCell;
        float maxZ = worldCenter.z + halfCell;

        float normMinX = Mathf.Clamp01((minX - terrainPos.x) / size.x);
        float normMaxX = Mathf.Clamp01((maxX - terrainPos.x) / size.x);
        float normMinZ = Mathf.Clamp01((minZ - terrainPos.z) / size.z);
        float normMaxZ = Mathf.Clamp01((maxZ - terrainPos.z) / size.z);

        int detailWidth = data.detailWidth;
        int detailHeight = data.detailHeight;

        int x0 = Mathf.Clamp((int)(normMinX * detailWidth), 0, detailWidth - 1);
        int x1 = Mathf.Clamp((int)(normMaxX * detailWidth), 0, detailWidth - 1);
        int y0 = Mathf.Clamp((int)(normMinZ * detailHeight), 0, detailHeight - 1);
        int y1 = Mathf.Clamp((int)(normMaxZ * detailHeight), 0, detailHeight - 1);

        xBase = x0;
        yBase = y0;
        width = Mathf.Max(1, x1 - x0 + 1);
        height = Mathf.Max(1, y1 - y0 + 1);
    }
}
