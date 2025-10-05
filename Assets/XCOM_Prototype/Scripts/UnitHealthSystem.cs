using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHealthSystem : MonoBehaviour {


    private HealthSystem healthSystem;

    private void Awake() {
        healthSystem = new HealthSystem(100);
    }

}