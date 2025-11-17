using System.Collections.Generic;
using UnityEngine;

public class ExtractionArea : MonoBehaviour
{
    //[SerializeField] private List<Unit> unitList;
    [SerializeField] private int unitCount;
    void OnTriggerEnter(Collider other)
    {
        Unit tempUnit = other.GetComponent<Unit>();
        Debug.Log($"{tempUnit.name}");
        if(!tempUnit.IsEnemy())
        {
            unitCount++;
            //unitList.Add(tempUnit);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if(unitCount == UnitManager.Instance.friendlyUnitList.Count)
        {
            LevelManager.Instance.LoadLevel("HUB");
        }
    }

    void OnTriggerExit(Collider other)
    {
        Unit tempUnit = other.GetComponent<Unit>();
        if(!tempUnit.IsEnemy())
        {
            unitCount--;
        }
    }
}