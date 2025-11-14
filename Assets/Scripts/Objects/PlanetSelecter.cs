using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlanetSelecter : MonoBehaviour
{
    public List<Transform> areas;
    [Range(0.1f, 10f)] public float rotateSpeed = 5f;
    public Transform cam;
    public List<Material> colorPoints; // 0 - Deselected | 1 - Selected

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
            currentIndex = (currentIndex + 1) % areas.Count;

        if (Input.GetKeyDown(KeyCode.S))
            currentIndex = (currentIndex - 1 + areas.Count) % areas.Count;

        if (Input.GetKeyDown(KeyCode.A))
            currentIndex = (currentIndex - 1 + areas.Count) % areas.Count;

        if (Input.GetKeyDown(KeyCode.D))
            currentIndex = (currentIndex + 1) % areas.Count;
    }

    void RotatePlanet()
    {
        Transform target = areas[currentIndex];

        ClearColor();
        SetColor(target);
        
        Vector3 dir = (target.position - transform.position).normalized;
        Vector3 targetForward = (cam.position - transform.position).normalized;
        Quaternion lookRot = Quaternion.FromToRotation(dir, targetForward) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * rotateSpeed);
    }

    void ClearColor()
    {
        for(int i = 0; i < areas.Count; i++)
        {
            areas[i].GetComponent<MeshRenderer>().material = colorPoints[0];
        }
    }

    void SetColor(Transform target)
    {
        target.GetComponent<MeshRenderer>().material = colorPoints[1];
    }
}
