using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridVisual : MonoBehaviour {

    public static GridVisual Instance { get; private set; }


    public enum GridVisualType {
        None,
        White,
        Blue,
        Red,
        RedSoft,
        Yellow,
    }


    [SerializeField] private Transform pfGridVisual;
    [SerializeField] private Material gridWhiteMaterial;
    [SerializeField] private Material gridBlueMaterial;
    [SerializeField] private Material gridRedMaterial;
    [SerializeField] private Material gridRedSoftMaterial;
    [SerializeField] private Material gridYellowMaterial;


    private int width;
    private int height;
    private MeshRenderer[,] gridVisualArray;


    private void Start() {
        Instance = this;

        LevelGrid.Instance.GetWidthHeight(out width, out height);

        gridVisualArray = new MeshRenderer[width, height];

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                Transform gridVisual = Instantiate(pfGridVisual, LevelGrid.Instance.GetWorldPosition(x, y), Quaternion.identity);
                gridVisual.parent = transform;
                gridVisualArray[x, y] = gridVisual.Find("Visual").GetComponent<MeshRenderer>();
            }
        }
    }

    // Hide all Grid Positions
    public void HideAllGridPositions() {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                MeshRenderer gridVisualMeshRenderer = gridVisualArray[x, y];
                gridVisualMeshRenderer.enabled = false;
            }
        }
    }

    // Show all Grid Positions within this Range
    public void ShowGridPositionsRange(Vector2Int gridPosition, int range, GridVisualType gridVisualType) {
        List<Vector2Int> gridPositionList = new List<Vector2Int>();

        for (int x = -range; x <= range; x++) {
            for (int y = -range; y <= range; y++) {
                Vector2Int testGridPosition = gridPosition + new Vector2Int(x, y);
                Vector2Int testVector = testGridPosition - gridPosition;
                int testDistance = Mathf.Abs(testVector.x) + Mathf.Abs(testVector.y);
                if (testDistance <= range) {
                    // Within valid move range
                    if (LevelGrid.Instance.IsValidGridPosition(testGridPosition)) {
                        // Valid grid position (not off bounds)
                        gridPositionList.Add(testGridPosition);
                    }
                }
            }
        }

        ShowGridPositions(gridPositionList, gridVisualType);
    }

    // Show all these Grid positions with this GridVisualType
    public void ShowGridPositions(List<Vector2Int> gridPositionList, GridVisualType gridVisualType) {
        foreach (Vector2Int gridPosition in gridPositionList) {
            MeshRenderer gridVisualMeshRenderer = gridVisualArray[gridPosition.x, gridPosition.y];
            gridVisualMeshRenderer.enabled = true;
            gridVisualMeshRenderer.material = GetGridMaterial(gridVisualType);
        }
    }

    // Convet GridVisualType into GridMaterial
    private Material GetGridMaterial(GridVisualType gridVisualType) {
        switch (gridVisualType) {
            default:
            case GridVisualType.None:
            case GridVisualType.White:  return gridWhiteMaterial;

            case GridVisualType.Yellow: return gridYellowMaterial;
            case GridVisualType.Blue:   return gridBlueMaterial;
            case GridVisualType.Red:    return gridRedMaterial;
            case GridVisualType.RedSoft:return gridRedSoftMaterial;
        }
    }

}