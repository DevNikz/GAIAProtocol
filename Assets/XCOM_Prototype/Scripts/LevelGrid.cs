using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour {

    public static LevelGrid Instance { get; private set; }


    [SerializeField] private LayerMask pathfindingObstaclesLayerMask;


    private GridXZ<GridPosition> grid;


    private void Awake() {
        Instance = this;

        grid = new GridXZ<GridPosition>(30, 30, 2f, Vector3.zero, (GridXZ<GridPosition> g, int x, int y) => new GridPosition(g, x, y));

        // Setup level obstacles
        LevelGrid.Instance.GetWidthHeight(out int width, out int height);
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                Vector3 worldPosition = LevelGrid.Instance.GetWorldPosition(x, y);

                if (Physics.Raycast(worldPosition + Vector3.down * 5f, Vector3.up, out RaycastHit raycastHit, 10f, pathfindingObstaclesLayerMask)) {
                    // Something at this position, cover?
                    if (raycastHit.collider.TryGetComponent(out CoverObject coverObject)) {
                        grid.GetGridObject(x, y).SetCoverType(coverObject.GetCoverType());
                    }
                }
            }
        }
    }


    public class GridPosition {

        private GridXZ<GridPosition> grid;
        private int x;
        private int y;
        private List<Unit> unitList; // Units at this position
        private CoverType coverType;

        public GridPosition(GridXZ<GridPosition> grid, int x, int y) {
            this.grid = grid;
            this.x = x;
            this.y = y;
            unitList = new List<Unit>();
        }

        public void AddUnit(Unit unit) {
            unitList.Add(unit);
            grid.TriggerGridObjectChanged(x, y);
        }

        public void RemoveUnit(Unit unit) {
            unitList.Remove(unit);
            grid.TriggerGridObjectChanged(x, y);
        }

        public Unit GetUnit() {
            if (unitList.Count > 0) {
                return unitList[0];
            } else {
                return null;
            }
        }

        public void SetCoverType(CoverType coverType) {
            this.coverType = coverType;
        }

        public CoverType GetCoverType() {
            return coverType;
        }

        public override string ToString() {
            return GetUnit() != null ? GetUnit().ToString() : "0";
        }

    }

    public float GetCellSize() => grid.GetCellSize();

    public bool IsValidGridPosition(Vector2Int gridPosition) => grid.IsValidGridPosition(gridPosition);

    public Vector2Int GetGridPosition(Vector3 worldPosition) {
        grid.GetXZ(worldPosition, out int x, out int z);
        return new Vector2Int(x, z);
    }

    public Vector3 SnapWorldPosition(Vector3 worldPosition) {
        Vector2Int gridPosition = GetGridPosition(worldPosition);
        return GetWorldPosition(gridPosition);
    }

    public Vector3 GetWorldPosition(Vector2Int gridPosition) {
        return grid.GetWorldPosition(gridPosition.x, gridPosition.y);
    }

    public Vector3 GetWorldPosition(int x, int y) {
        return grid.GetWorldPosition(x, y);
    }

    public CoverType GetCoverTypeAtPosition(Vector3 worldPosition) {
        return grid.GetGridObject(worldPosition).GetCoverType();
    }

    public CoverType GetUnitCoverType(Vector3 worldPosition) {
        // Find closest cover to this position
        grid.GetXZ(worldPosition, out int x, out int z);

        bool hasLeft    = grid.IsValidGridPosition(new Vector2Int(x - 1, z + 0));
        bool hasRight   = grid.IsValidGridPosition(new Vector2Int(x + 1, z + 0));
        bool hasFront   = grid.IsValidGridPosition(new Vector2Int(x + 0, z + 1));
        bool hasBack    = grid.IsValidGridPosition(new Vector2Int(x + 0, z - 1));

        CoverType leftCover, rightCover, frontCover, backCover;
        leftCover = rightCover = frontCover = backCover = CoverType.None;

        if (hasLeft)    leftCover     = grid.GetGridObject(x - 1, z + 0).GetCoverType();
        if (hasRight)   rightCover    = grid.GetGridObject(x + 1, z + 0).GetCoverType();
        if (hasFront)   frontCover    = grid.GetGridObject(x + 0, z + 1).GetCoverType();
        if (hasBack)    backCover     = grid.GetGridObject(x + 0, z - 1).GetCoverType();

        if (leftCover == CoverType.Full ||
            rightCover == CoverType.Full ||
            frontCover == CoverType.Full ||
            backCover == CoverType.Full) {
            // At least one Full Cover
            return CoverType.Full;
        }

        if (leftCover == CoverType.Half ||
            rightCover == CoverType.Half ||
            frontCover == CoverType.Half ||
            backCover == CoverType.Half) {
            // At least one Half Cover
            return CoverType.Half;
        }

        return CoverType.None;
    }

    public void SetUnitAtGridPosition(Vector2Int gridPosition, Unit unit) {
        grid.GetGridObject(gridPosition.x, gridPosition.y).AddUnit(unit);
    }

    public void ClearUnitAtGridPosition(Vector2Int gridPosition, Unit unit) {
        grid.GetGridObject(gridPosition.x, gridPosition.y).RemoveUnit(unit);
    }

    public Unit GetUnit(Vector2Int gridPosition) {
        return grid.GetGridObject(gridPosition.x, gridPosition.y).GetUnit();
    }

    public bool HasUnitAtPosition(Vector2Int gridPosition) {
        return GetUnit(gridPosition) != null;
    }

    public void UnitMovedGridPosition(Unit unit, Vector2Int fromGridPosition, Vector2Int toGridPosition) {
        grid.GetGridObject(fromGridPosition.x, fromGridPosition.y).RemoveUnit(unit);

        grid.GetGridObject(toGridPosition.x, toGridPosition.y).AddUnit(unit);
    }

    public void GetWidthHeight(out int width, out int height) {
        width = grid.GetWidth();
        height = grid.GetHeight();
    }

    public List<Vector3> GetPath(Vector3 fromPosition, Vector3 toPosition) {
        return LevelPathfinding.Instance.FindPath(fromPosition, toPosition, out int pathLength);

        grid.GetXZ(fromPosition, out int fromX, out int fromZ);
        grid.GetXZ(toPosition, out int toX, out int toZ);

        //Debug.Log($"{fromX}, {fromZ} -> {toX}, {toZ}");

        List<Vector3> pathList = new List<Vector3>();

        while (toX > fromX) {
            fromX++;
            pathList.Add(grid.GetWorldPosition(fromX, fromZ));
        }
        while (toX < fromX) {
            fromX--;
            pathList.Add(grid.GetWorldPosition(fromX, fromZ));
        }
        while (toZ > fromZ) {
            fromZ++;
            pathList.Add(grid.GetWorldPosition(fromX, fromZ));
        }
        while (toZ < fromZ) {
            fromZ--;
            pathList.Add(grid.GetWorldPosition(fromX, fromZ));
        }

        return pathList;
    }

}