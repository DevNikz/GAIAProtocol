using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWarVisual : MonoBehaviour {

    public static FogOfWarVisual Instance { get; private set; }


    [SerializeField] private Transform pfFogOfWarGridVisual;


    private int width;
    private int height;
    private MeshRenderer[,] gridVisualArray;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        LevelGrid.Instance.GetWidthHeight(out width, out height);

        gridVisualArray = new MeshRenderer[width, height];

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                Transform gridVisual = Instantiate(pfFogOfWarGridVisual, LevelGrid.Instance.GetWorldPosition(x, y), Quaternion.identity);
                gridVisual.parent = transform;
                gridVisualArray[x, y] = gridVisual.Find("Visual").GetComponent<MeshRenderer>();
            }
        }
    }

    // Hide all Grid Positions, which means show the shadow visual
    public void HideAllGridPositions() {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                MeshRenderer gridVisualMeshRenderer = gridVisualArray[x, y];
                gridVisualMeshRenderer.enabled = true;
            }
        }
    }

    // Show all these Grid positions which means hide the shadow visual
    public void ShowGridPositions(List<Vector2Int> gridPositionList) {
        foreach (Vector2Int gridPosition in gridPositionList) {
            MeshRenderer gridVisualMeshRenderer = gridVisualArray[gridPosition.x, gridPosition.y];
            gridVisualMeshRenderer.enabled = false;
        }
    }


}