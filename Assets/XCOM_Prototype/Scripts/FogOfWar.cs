using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class FogOfWar : MonoBehaviour {

    public static FogOfWar Instance { get; private set; }


    private void Start() {
        UnitManager.Instance.OnAnyUnitMovedGridPosition += UnitManager_OnAnyUnitMovedGridPosition;
        UpdateAllFogOfWar();
    }

    private void UnitManager_OnAnyUnitMovedGridPosition(object sender, System.EventArgs e) {
        UpdateAllFogOfWar();
    }

    private void UpdateAllFogOfWar() {
        FogOfWarVisual.Instance.HideAllGridPositions();

        List<Vector2Int> revealedGridPositionList = new List<Vector2Int>();

        foreach (Unit unit in UnitManager.Instance.GetFriendlyUnitList()) {
            Vector2Int unitGridPosition = unit.GetGridPosition();
            Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(unitGridPosition);

            revealedGridPositionList.Add(unitGridPosition);

            TestFogOfWarOnPosition(unitWorldPosition, ref revealedGridPositionList);

            // Test positions around Unit
            List<Vector2Int> neighbourGridOffsetList = new List<Vector2Int> {
                new Vector2Int(+1, +0),
                new Vector2Int(-1, +0),
                new Vector2Int(+0, +1),
                new Vector2Int(+0, -1),

                new Vector2Int(+1, +1),
                new Vector2Int(-1, +1),
                new Vector2Int(+1, -1),
                new Vector2Int(-1, -1),
            };

            foreach (Vector2Int neighbourGridOffset in neighbourGridOffsetList) {
                Vector2Int neighbourGridPosition = unitGridPosition + neighbourGridOffset;
                if (LevelGrid.Instance.IsValidGridPosition(neighbourGridPosition)) {
                    // Valid
                    Vector3 neighbourWorldPosition = LevelGrid.Instance.GetWorldPosition(neighbourGridPosition);
                    TestFogOfWarOnPosition(neighbourWorldPosition, ref revealedGridPositionList);
                }
            }
        }

        FogOfWarVisual.Instance.ShowGridPositions(revealedGridPositionList);

        // Show/Hide all Enemies in Revealed Grid Positions
        foreach (Unit enemyUnit in UnitManager.Instance.GetEnemyUnitList()) {
            if (revealedGridPositionList.Contains(enemyUnit.GetGridPosition())) {
                // Enemy visible
                enemyUnit.ShowVisual();
            } else {
                // Enemy invisible
                enemyUnit.HideVisual();
            }
        }
    }

    private void TestFogOfWarOnPosition(Vector3 unitWorldPosition, ref List<Vector2Int> revealedGridPositionList) {
        Vector3 baseDir = new Vector3(1, 0, 0);
        float angleIncrease = 10;
        for (float angle = 0; angle < 360; angle += angleIncrease) {
            Vector3 dir = UtilsClass.ApplyRotationToVectorXZ(baseDir, angle);
            //Debug.DrawLine(unitWorldPosition, unitWorldPosition + dir * 14f, Color.green, 5f);

            float viewDistanceMax = 14f;
            float viewDistanceIncrease = .4f;
            for (float viewDistance = 0f; viewDistance < viewDistanceMax; viewDistance += viewDistanceIncrease) {
                Vector3 targetPosition = unitWorldPosition + dir * viewDistance;
                Vector2Int targetGridPosition = LevelGrid.Instance.GetGridPosition(targetPosition);
                if (LevelGrid.Instance.IsValidGridPosition(targetGridPosition)) {
                    // Valid Grid Position
                    CoverType coverType = LevelGrid.Instance.GetCoverTypeAtPosition(targetPosition);
                    if (coverType == CoverType.Full) break; // If coverType is Full, Unit cannot see further through it

                    if (!revealedGridPositionList.Contains(targetGridPosition)) {
                        // Position not yet tested
                        revealedGridPositionList.Add(targetGridPosition);
                    }
                }
            }
        }
    }
}