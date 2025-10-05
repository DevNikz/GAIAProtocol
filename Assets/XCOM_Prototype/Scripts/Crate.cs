using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour {

    [SerializeField] private Transform pfCrateExploded;

    public void Damage() {
        Transform crateExplodedTransform = Instantiate(pfCrateExploded, transform.position, Quaternion.identity);

        foreach (Transform child in crateExplodedTransform) {
            if (child.TryGetComponent(out Rigidbody rigidbody)) {
                rigidbody.AddExplosionForce(300f, transform.position, 10f);
            }
        }

        Destroy(gameObject);
    }

}