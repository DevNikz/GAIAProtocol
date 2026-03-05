using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitScene : MonoBehaviour
{
    void Awake()
    {
        UnitManager.Instance.ClearRefList();
        UnitManager.Instance.SetReferenceList(transform.Cast<Transform>().ToList());
    }
}