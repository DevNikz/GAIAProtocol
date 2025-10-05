using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathfindingXCOM;
using CodeMonkey.Utils;

public class XCOM_Prototype : MonoBehaviour {


    [SerializeField] private Unit unit;



    private void Start() {
        unit.OnUnitActionCompleted += Unit_OnUnitActionCompleted;
    }

    private void Unit_OnUnitActionCompleted(object sender, IUnitAction e) {
        //Debug.Log("OnUnitActionCompleted " + e);
    }

    private void Update() {
        /*
        if (Input.GetMouseButtonDown(0)) {
            Vector3 targetPosition = Mouse3D.GetMouseWorldPosition();// unit.transform.position + new Vector3(3, 0, 1) * LevelGrid.Instance.GetCellSize();
            unit.GetComponent<MoveAction>().Move(LevelGrid.Instance.GetPath(unit.transform.position, targetPosition),
                () => Debug.Log("Move Complete"));
        }
        */
    }

}