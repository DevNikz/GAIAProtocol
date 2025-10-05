using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathfindingXCOM;

public class LevelPathfinding : MonoBehaviour {

    public static LevelPathfinding Instance { get; private set; }


    [SerializeField] private LayerMask pathfindingObstaclesLayerMask;

    private Pathfinding pathfinding;


    private void Awake() {
        Instance = this;
    }

    private void Start() {
        // Setup Pathfinding
        pathfinding = new Pathfinding(30, 30);

        // Setup level obstacles
        LevelGrid.Instance.GetWidthHeight(out int width, out int height);
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                Vector3 worldPosition = LevelGrid.Instance.GetWorldPosition(x, y);

                if (Physics.Raycast(worldPosition + Vector3.down * 5f, Vector3.up, 10f, pathfindingObstaclesLayerMask)) {
                    // Something at this position, unwalkable
                    pathfinding.GetGrid().GetGridObject(ConvertXZtoXY(worldPosition)).SetIsWalkable(false);
                }
            }
        }

        //List<Vector3> pathXY = pathfinding.FindPath(ConvertXZtoXY(new Vector3(4, 0, 4)), ConvertXZtoXY(new Vector3(6, 0, 2)), out int pathLength);
    }

    private Vector3 ConvertXZtoXY(Vector3 position) {
        return new Vector3(position.x, position.z);
    }

    public List<Vector3> FindPath(Vector3 startPosition, Vector3 endPosition, out int pathLength) {
        List<Vector3> pathXY = pathfinding.FindPath(ConvertXZtoXY(startPosition), ConvertXZtoXY(endPosition), out pathLength);

        if (pathXY == null) {
            // No path
            return null;
        }

        List<Vector3> pathXZ = new List<Vector3>();

        foreach (Vector3 pathPosition in pathXY) {
            pathXZ.Add(new Vector3(pathPosition.x, 0, pathPosition.y));
        }

        return pathXZ;
    }

}