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

    [Header("Areas")]
    public List<Material> colorPoints; // 0 - Deselected | 1 - Selected
    public List<Material> colorArea; // 0 - Locked | 1 - Unlocked;
    [SerializeField] private int currentIndex = 0;

    //Mission Select UI
    [Header("Mission Select")]
    [SerializeField] private GameObject canvas;
    [SerializeField] private Image missionImage;
    [SerializeField] private TextMeshProUGUI missionHeader;
    [SerializeField] private TextMeshProUGUI missionDesc;
    [SerializeField] private GameObject button;
    [SerializeField] private List<Sprite> levelIcons;
    [SerializeField] private GameObject levelComplete;
    [SerializeField] private GameObject nullButton;

    [Header("Objectives")]
    [SerializeField] private List<GameObject> stateIcons;
    [SerializeField] private GameObject stateContainer;
    [SerializeField] private GameObject nullState;

    [Header("Rewards")]
    [SerializeField] private List<GameObject> rewardList;
    [SerializeField] private GameObject rewardContainer;
    [SerializeField] private GameObject nullReward;
    private bool hasAddedChildren;

    void Awake() {
        cam = Camera.main.transform;
        hasAddedChildren = false;
    }

    void Start()
    {
        ClearAreas();
        SetupArea(0);
        SetupArea(1);
    }

    void ClearAreas()
    {
        for(int i = 0; i < areas.Count; i++)
        {
            areas[i].parent.GetComponent<MeshRenderer>().material = colorArea[0];
        } 
    }

    void SetupArea(int index)
    {
        areas[index].parent.GetComponent<MeshRenderer>().material = colorArea[1];
    }

    void Update()
    {
        HandleInput();
        RotatePlanet();
        if(!hasAddedChildren) CheckUI();
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.W)) {
            currentIndex = (currentIndex + 1) % areas.Count;
            hasAddedChildren = false;
        }

        if (Input.GetKeyDown(KeyCode.S)) {
            currentIndex = (currentIndex - 1 + areas.Count) % areas.Count;
            hasAddedChildren = false;
        }

        if (Input.GetKeyDown(KeyCode.A)) {
            currentIndex = (currentIndex - 1 + areas.Count) % areas.Count;
            hasAddedChildren = false;
        }

        if (Input.GetKeyDown(KeyCode.D)) {
            currentIndex = (currentIndex + 1) % areas.Count;
            hasAddedChildren = false;
        }
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

                //button.GetComponent<ChangeLevelButton>().sceneName = "Forest 1";
                nullButton.SetActive(false);
                if(WorldManager.Instance.GetWorldComplete(currentIndex))
                {
                    levelComplete.SetActive(true);
                    button.SetActive(false);
                }  
                else {
                    levelComplete.SetActive(false);
                    button.SetActive(true);
                    button.GetComponent<ChangeLevelButton>().sceneIndex = currentIndex+1;
                }

                //Add objective details later on
                nullState.SetActive(false);
                DestroyChildren(stateContainer);
                AddChildren(stateIcons[currentIndex], stateContainer);

                nullReward.SetActive(false);
                DestroyChildren(rewardContainer);
                AddChildren(rewardList[currentIndex], rewardContainer);
                hasAddedChildren = true;

                //Set objectives first
                ObjectiveManager.Instance.ClearObjectives();
                ObjectiveManager.Instance.AddObjective(new ObjectiveObject(missionDesc.text));

                //Set Level
                LevelManager.Instance.SetCurrentLevel(currentIndex + 1);

                //Set Max Prompted Points on Completion
                CurrencyManager.Instance.SetPromptedPoints(1);
                break;
            case 1:
                canvas.SetActive(true);
                missionImage.sprite = levelIcons[currentIndex];
                missionHeader.text = "Operation\nStop the leakage";
                missionDesc.text = "Repair the factory's pipeline to stop the leakage.";

                nullButton.SetActive(false);
                if(WorldManager.Instance.GetWorldComplete(currentIndex))
                {
                    levelComplete.SetActive(true);
                    button.SetActive(false);
                }  
                else {
                    levelComplete.SetActive(false);
                    button.SetActive(true);
                    button.GetComponent<ChangeLevelButton>().sceneIndex = currentIndex+1;
                }

                //Add objective details later on
                nullState.SetActive(false);
                DestroyChildren(stateContainer);
                AddChildren(stateIcons[0], stateContainer);

                nullReward.SetActive(false);
                DestroyChildren(rewardContainer);
                AddChildren(rewardList[0], rewardContainer);
                hasAddedChildren = true;
                //Set objectives first
                ObjectiveManager.Instance.ClearObjectives();
                ObjectiveManager.Instance.AddObjective(new ObjectiveObject(missionDesc.text));

                //Set Level
                LevelManager.Instance.SetCurrentLevel(currentIndex + 1);

                //Set Max Prompted Points on Completion
                CurrencyManager.Instance.SetPromptedPoints(1);
                break;
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
            case 8:
            case 9:
                canvas.SetActive(true);
                missionImage.sprite = levelIcons[currentIndex];
                missionHeader.text = "Locked Operation";
                missionDesc.text = "Classified Data.";

                levelComplete.SetActive(false);
                button.SetActive(false);
                nullButton.SetActive(true);

                //State
                DestroyChildren(stateContainer);
                nullState.SetActive(true);

                //Reward
                DestroyChildren(rewardContainer);
                nullReward.SetActive(true);
                break;
            default:
                canvas.SetActive(false);
                break;
        }
    }

    void DestroyChildren(GameObject parentObject)
    {
        for (int i = parentObject.transform.childCount - 1; i >= 0; i--)
        {
            if(parentObject.name != parentObject.transform.GetChild(i).name) Destroy(parentObject.transform.GetChild(i).gameObject);
        }
    }

    void AddChildren(GameObject inst, GameObject parentObj)
    {
        GameObject obj = Instantiate(inst, parentObj.transform);
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
