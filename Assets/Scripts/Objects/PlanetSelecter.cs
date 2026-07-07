using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlanetSelecter : MonoBehaviour
{
    public List<Transform> areas;

    [Range(0.1f, 10f)]
    public float rotateSpeed = 5f;
    public Transform cam;

    [Header("Areas")]
    public List<Material> colorPoints; // 0 - Deselected | 1 - Selected
    public List<Material> colorArea; // 0 - Locked | 1 - Unlocked;

    [SerializeField]
    private int currentIndex = 0;

    [Header("Planet")]
    [SerializeField]
    private int currentPlanetIndex = 0;

    //Mission Select UI
    [Header("Mission Select")]
    [SerializeField]
    private GameObject canvas;

    [SerializeField]
    private Image missionImage;

    [SerializeField]
    private TextMeshProUGUI missionHeader;

    [SerializeField]
    private TextMeshProUGUI missionDesc;

    [SerializeField]
    private GameObject button;

    [SerializeField]
    private List<Sprite> levelIcons;

    [SerializeField]
    private GameObject levelComplete;

    [SerializeField]
    private GameObject nullButton;

    [Header("Objectives")]
    [SerializeField]
    private List<GameObject> stateIcons;

    [SerializeField]
    private GameObject stateContainer;

    [SerializeField]
    private GameObject nullState;

    [Header("Rewards")]
    [SerializeField]
    private List<GameObject> rewardList;

    [SerializeField]
    private GameObject rewardContainer;

    [SerializeField]
    private GameObject nullReward;
    private bool hasAddedChildren;

    void Awake()
    {
        cam = Camera.main.transform;
        hasAddedChildren = false;
    }

    void Start()
    {
        ClearAreas();
        SetupCorruptedArea(0);

        if (WorldManager.Instance.GetUnlockStateIndex(1))
            SetupCorruptedArea(1);
        else
            SetupLockedArea(1);

        if (WorldManager.Instance.GetUnlockStateIndex(1))
            SetupCorruptedArea(2);
        else
            SetupLockedArea(2);
    }

    void ClearAreas()
    {
        for (int i = 0; i < areas.Count; i++)
        {
            areas[i].parent.GetComponent<MeshRenderer>().material = colorArea[0];
        }
    }

    void SetupLockedArea(int index)
    {
        areas[index].parent.GetComponent<MeshRenderer>().material = colorArea[0];
    }

    void SetupClearedArea(int index)
    {
        areas[index].parent.GetComponent<MeshRenderer>().material = colorArea[1];
    }

    void SetupCorruptedArea(int index)
    {
        areas[index].parent.GetComponent<MeshRenderer>().material = colorArea[2];
    }

    void Update()
    {
        if (!InputManager.Instance.AreLegacyInputsDisabled())
            HandleAreaInput();

        RotatePlanet();

        if (!hasAddedChildren)
            CheckUI();
    }

    public void SwitchPlanet()
    {
        if (currentPlanetIndex == 0)
        {
            currentPlanetIndex = 1;
            CorruptionManager.Instance.SetAreaIndex(1);
        }
        else
        {
            currentPlanetIndex = 0;
            CorruptionManager.Instance.SetAreaIndex(0);
        }
    }

    void HandleAreaInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            currentIndex = (currentIndex + 1) % areas.Count;
            hasAddedChildren = false;
            SoundManager.Instance.PlaySFX("Select Planet");
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            currentIndex = (currentIndex - 1 + areas.Count) % areas.Count;
            hasAddedChildren = false;
            SoundManager.Instance.PlaySFX("Select Planet");
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            currentIndex = (currentIndex - 1 + areas.Count) % areas.Count;
            hasAddedChildren = false;
            SoundManager.Instance.PlaySFX("Select Planet");
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            currentIndex = (currentIndex + 1) % areas.Count;
            hasAddedChildren = false;
            SoundManager.Instance.PlaySFX("Select Planet");
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
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            lookRot,
            Time.deltaTime * rotateSpeed
        );
    }

    void CheckUI()
    {
        switch (currentIndex)
        {
            case 0:
                canvas.SetActive(true);
                missionImage.sprite = levelIcons[currentIndex];
                missionHeader.text = "Operation\nActivate The Satellite Array";
                missionDesc.text = "Activate the array to establish communications to HQ.";

                //button.GetComponent<ChangeLevelButton>().sceneName = "Forest 1";
                nullButton.SetActive(false);
                if (WorldManager.Instance.GetWorldComplete(currentIndex))
                {
                    levelComplete.SetActive(true);
                    button.SetActive(false);
                    SetupClearedArea(0);
                }
                else
                {
                    levelComplete.SetActive(false);
                    button.SetActive(true);
                    button.GetComponent<ChangeLevelButton>().sceneIndex = currentIndex + 1;
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
                //ObjectiveManager.Instance.ClearObjectives();
                //ObjectiveManager.Instance.AddObjective(new ObjectiveObject(missionDesc.text));

                //Set Level
                LevelManager.Instance.SetCurrentLevel(currentIndex + 1);

                //Set Max Prompted Points on Completion
                CurrencyManager.Instance.SetPromptedPoints(10);
                RewardsManager.Instance.SetPoints(10);
                CorruptionManager.Instance.SetPromptedCorruption(0.52f);
                CorruptionManager.Instance.SetPromptedCorruptionIndex(0);
                break;
            /*
            case 1:
                if(WorldManager.Instance.GetUnlockStateIndex(1) == false)
                {
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
                    CurrencyManager.Instance.SetPromptedPoints(0);
                    RewardsManager.Instance.SetPoints(0);
                    CorruptionManager.Instance.SetPromptedCorruption(0f);
                    CorruptionManager.Instance.SetPromptedCorruptionIndex(0);
                }
                else
                {
                    //Lock
                    canvas.SetActive(true);
                    missionImage.sprite = levelIcons[currentIndex];
                    missionHeader.text = "Operation\nShutdown The Facility";
                    missionDesc.text = "Shut down The Former's facility to prevent the corruption from spreading out.";

                    //button.GetComponent<ChangeLevelButton>().sceneName = "Forest 1";
                    nullButton.SetActive(false);
                    if(WorldManager.Instance.GetWorldComplete(currentIndex))
                    {
                        levelComplete.SetActive(true);
                        button.SetActive(false);
                        SetupClearedArea(1);
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
                    CurrencyManager.Instance.SetPromptedPoints(15);
                    RewardsManager.Instance.SetPoints(15);
                    CorruptionManager.Instance.SetPromptedCorruption(0.7f);
                    CorruptionManager.Instance.SetPromptedCorruptionIndex(0);
                }
                
                break;
            */
            case 1:
            case 2:
                //Lock this first
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
                CurrencyManager.Instance.SetPromptedPoints(0);
                RewardsManager.Instance.SetPoints(0);
                CorruptionManager.Instance.SetPromptedCorruption(0f);
                CorruptionManager.Instance.SetPromptedCorruptionIndex(0);
                /*
                if(WorldManager.Instance.GetUnlockStateIndex(2) == false)
                {
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
                    CurrencyManager.Instance.SetPromptedPoints(0);
                    CorruptionManager.Instance.SetPromptedCorruption(1f);
                    CorruptionManager.Instance.SetPromptedCorruptionIndex(0);
                }
                else
                {
                    canvas.SetActive(true);
                    missionImage.sprite = levelIcons[currentIndex];
                    missionHeader.text = "Operation\nChelonia";
                    missionDesc.text = "Subdue The Chelonia.";

                    //button.GetComponent<ChangeLevelButton>().sceneName = "Forest 1";
                    nullButton.SetActive(false);
                    if(WorldManager.Instance.GetWorldComplete(currentIndex))
                    {
                        levelComplete.SetActive(true);
                        button.SetActive(false);
                        SetupClearedArea(1);
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
                    CurrencyManager.Instance.SetPromptedPoints(20);
                    CorruptionManager.Instance.SetPromptedCorruption(1f);
                    CorruptionManager.Instance.SetPromptedCorruptionIndex(0);
                }
                */
                break;
            default:
                canvas.SetActive(false);
                break;

            /*
            //Locked
            case 2:
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
                CurrencyManager.Instance.SetPromptedPoints(20);
                CorruptionManager.Instance.SetPromptedCorruption(1f);
                CorruptionManager.Instance.SetPromptedCorruptionIndex(0);
                break;
            */
        }
    }

    void DestroyChildren(GameObject parentObject)
    {
        for (int i = parentObject.transform.childCount - 1; i >= 0; i--)
        {
            if (parentObject.name != parentObject.transform.GetChild(i).name)
                Destroy(parentObject.transform.GetChild(i).gameObject);
        }
    }

    void AddChildren(GameObject inst, GameObject parentObj)
    {
        GameObject obj = Instantiate(inst, parentObj.transform);
    }

    void ClearColor()
    {
        for (int i = 0; i < areas.Count; i++)
        {
            areas[i].GetComponent<MeshRenderer>().material = colorPoints[0];
        }
    }

    void SetColor(Transform target)
    {
        target.GetComponent<MeshRenderer>().material = colorPoints[1];
    }
}
