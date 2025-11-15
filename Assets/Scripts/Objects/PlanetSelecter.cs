using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlanetSelecter : MonoBehaviour
{
    public List<Transform> areas;
    [Range(0.1f, 10f)] public float rotateSpeed = 5f;
    public Transform cam;
    public List<Material> colorPoints; // 0 - Deselected | 1 - Selected

    [SerializeField] private int currentIndex = 0;

    //Mission Select UI
    [SerializeField] private GameObject canvas;
    [SerializeField] private Image missionImage;
    [SerializeField] private TextMeshProUGUI missionHeader;
    [SerializeField] private TextMeshProUGUI missionDesc;
    [SerializeField] private GameObject button;
    [SerializeField] private List<Sprite> levelIcons;

    void Awake() {
        cam = Camera.main.transform;
    }

    void Update()
    {
        HandleInput();
        RotatePlanet();
        CheckUI();
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

    void CheckUI()
    {
        switch(currentIndex)
        {
            case 0:
                canvas.SetActive(true);
                missionImage.sprite = levelIcons[currentIndex];
                missionHeader.text = "Operation\nActivate The Facility";
                missionDesc.text = "Activate Gaia Infrastructure to establish communications to HQ.";
                button.GetComponent<ChangeLevelButton>().sceneName = "Forest 1";

                //Add objective details later on

                //Set objectives first
                ObjectiveManager.Instance.ClearObjectives();
                ObjectiveManager.Instance.AddObjective(new ObjectiveObject(missionDesc.text));
                break;
            case 1:
                canvas.SetActive(true);
                missionImage.sprite = levelIcons[currentIndex];
                missionHeader.text = "Operation\nGather Waste Piles";
                missionDesc.text = "Gather the waste piles to the dumping site.";
                break;
            default:
                canvas.SetActive(false);
                break;
        }
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
