using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtractionArea : MonoBehaviour
{
    //[SerializeField] private List<Unit> unitList;
    [SerializeField] private int unitCount;
    void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Unit>() != null)
        {
            Unit tempUnit = other.GetComponent<Unit>();
            Debug.Log($"{tempUnit.name} has entered extraction");
            if(!tempUnit.IsEnemy())
            {
                unitCount++;
            }
        }
    }

    void OnTriggerStay()
    {
        if(unitCount == UnitManager.Instance.friendlyUnitList.Count)
        {
            ExtractionManager.Instance.SetButtonVisible(true);
            //LevelManager.Instance.LoadLevel("HUB");
            //StartCoroutine(StartExtraction());
        }
        else
        {
            ExtractionManager.Instance.SetButtonVisible(false);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.GetComponent<Unit>() != null)
        {
            Unit tempUnit = other.GetComponent<Unit>();
            Debug.Log($"{tempUnit.name} has left extraction");
            if(!tempUnit.IsEnemy())
            {
                unitCount--;
            }
        }
    }
}