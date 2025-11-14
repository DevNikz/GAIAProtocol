using System;
using UnityEngine;

public class PlanetSelecter : MonoBehaviour
{
    public Transform[] areas;
    [Range(0.1f, 10f)] public float rotateSpeed = 5f;
    public Transform cam;

    [SerializeField] private int currentIndex = 0;

    void Awake() {
        cam = Camera.main.transform;
    }

    void Update()
    {
        HandleInput();
        RotatePlanet();
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
            currentIndex = (currentIndex + 1) % areas.Length;

        if (Input.GetKeyDown(KeyCode.S))
            currentIndex = (currentIndex - 1 + areas.Length) % areas.Length;

        if (Input.GetKeyDown(KeyCode.A))
            currentIndex = (currentIndex - 1 + areas.Length) % areas.Length;

        if (Input.GetKeyDown(KeyCode.D))
            currentIndex = (currentIndex + 1) % areas.Length;
    }

    void RotatePlanet()
    {
        Transform target = areas[currentIndex];
        Vector3 dir = (target.position - transform.position).normalized;
        Vector3 targetForward = (cam.position - transform.position).normalized;
        Quaternion lookRot = Quaternion.FromToRotation(dir, targetForward) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * rotateSpeed);
    }
}
