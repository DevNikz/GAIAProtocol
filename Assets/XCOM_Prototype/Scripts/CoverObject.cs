using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverObject : MonoBehaviour {

    [SerializeField] private CoverType coverType;

    public CoverType GetCoverType() {
        return coverType;
    }

}

public enum CoverType {
    None,
    Half,
    Full
}
