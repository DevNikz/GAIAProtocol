using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class HUBTransitioner : MonoBehaviour
{
    public static HUBTransitioner Instance;

    [SerializeField]
    CinemachineVirtualCameraBase virtualCam;
    CinemachineOrbitalFollow pos;
    CinemachineRotationComposer rot;

    [SerializeField]
    GameObject missionSelect;

    [SerializeField]
    GameObject armory;

    [SerializeField]
    GameObject mechSelect;

    [SerializeField]
    GameObject toArmory;

    [SerializeField]
    GameObject planetLight;

    [SerializeField]
    bool firstTimeArmory = true;

    public void SetFirstTimeArmory(bool value)
    {
        firstTimeArmory = value;
    }

    [SerializeField]
    bool firstTimeDeployment = true;

    public void SetFirstTimeDeployment(bool value)
    {
        firstTimeDeployment = value;
    }

    Camera cam;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        cam = Camera.main;
        planetLight = GameObject.FindGameObjectWithTag("PlanetLight");
        missionSelect = GameObject.FindGameObjectWithTag("MissionSelecter");

        if (GameObject.FindGameObjectWithTag("Armory") != null)
        {
            armory = GameObject.FindGameObjectWithTag("Armory");
            armory.transform.Find("Content").gameObject.SetActive(false);
        }

        if (GameObject.FindGameObjectWithTag("MechSelect") != null)
        {
            mechSelect = GameObject.FindGameObjectWithTag("MechSelect");
            mechSelect.transform.Find("MainUI").gameObject.SetActive(false);
        }

        if (GameObject.FindGameObjectWithTag("ToArmory") != null)
        {
            toArmory = GameObject.FindGameObjectWithTag("ToArmory");
        }

        if (GameObject.FindGameObjectWithTag("VirtualCam") != null)
        {
            virtualCam = FindAnyObjectByType<CinemachineVirtualCameraBase>();
            pos = virtualCam.GetComponent<CinemachineOrbitalFollow>();
            rot = virtualCam.GetComponent<CinemachineRotationComposer>();
        }
    }

    void Start()
    {
        Intro();
    }

    IEnumerator BackToMissionSelect()
    {
        Debug.Log("Back To Mission Select");
        yield return new WaitForSeconds(0.5f);
        LevelManager.Instance.LoadLevelIndex(0);
        yield return new WaitForSeconds(1f);
        SoundManager.Instance.PlayMusic("HUB");
        FadeScreenManager.Instance.FadeOut();
        StartCoroutine(RewardHUD());
    }

    IEnumerator RewardHUD()
    {
        yield return new WaitForSeconds(0.5f);
        Debug.Log("RewardHUD: Show Canvas");
        RewardsManager.Instance.ShowCanvas();

        Debug.Log("RewardHUD: Animate Show");
        RewardsManager.Instance.AnimateShow();
        // yield return new WaitForSeconds(5f);

        // Debug.Log("RewardHUD: Animate Hide");
        // RewardsManager.Instance.AnimateHide();
    }

    public void Dead()
    {
        RewardsManager.Instance.SetRewardType(RewardsType.LOSE);
        FadeScreenManager.Instance.FadeIn();
        StartCoroutine(BackToMissionSelect());
    }

    public void ExtractForest1()
    {
        Debug.Log("Fade In");
        FadeScreenManager.Instance.FadeIn();
        StartCoroutine(SetupRewards_Forest1());
    }

    IEnumerator SetupRewards_Forest1()
    {
        Debug.Log("Setup Reward");
        yield return new WaitForSeconds(0.5f);

        RewardsManager.Instance.SetRewardType(RewardsType.WIN);
        RewardsManager.Instance.SetCurrentLevel(1);
        WorldManager.Instance.SetWorldComplete(true, LevelManager.Instance.GetCurrentLevel() - 1);
        WorldManager.Instance.SetUnlockStateIndex(LevelManager.Instance.GetCurrentLevel(), true);
        //CorruptionManager.Instance.SetCorruptionByIndex(0, 0.33f);
        yield return new WaitForSeconds(0.5f);

        StartCoroutine(BackToMissionSelect());
    }

    public void ExtractForest2()
    {
        FadeScreenManager.Instance.FadeIn();
        StartCoroutine(SetupRewards_Forest2());
    }

    IEnumerator SetupRewards_Forest2()
    {
        yield return new WaitForSeconds(0.5f);

        RewardsManager.Instance.SetRewardType(RewardsType.WIN);
        RewardsManager.Instance.SetCurrentLevel(1);
        WorldManager.Instance.SetWorldComplete(true, LevelManager.Instance.GetCurrentLevel() - 1);
        WorldManager.Instance.SetUnlockStateIndex(LevelManager.Instance.GetCurrentLevel(), true); //index start at 0
        RewardsManager.Instance.SetCurrentLevel(2);
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(BackToMissionSelect());
    }

    public void ExtractForest3()
    {
        FadeScreenManager.Instance.FadeIn();
        StartCoroutine(SetupRewards_Forest3());
    }

    IEnumerator SetupRewards_Forest3()
    {
        yield return new WaitForSeconds(0.5f);

        RewardsManager.Instance.SetRewardType(RewardsType.WIN);
        RewardsManager.Instance.SetCurrentLevel(3);
        WorldManager.Instance.SetWorldComplete(true, LevelManager.Instance.GetCurrentLevel() - 1);
        WorldManager.Instance.SetUnlockStateIndex(LevelManager.Instance.GetCurrentLevel(), true);
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(BackToMissionSelect());
    }

    public void ExtractForest4()
    {
        FadeScreenManager.Instance.FadeIn();
        StartCoroutine(SetupRewards_Forest4());
    }

    IEnumerator SetupRewards_Forest4()
    {
        yield return new WaitForSeconds(0.5f);

        RewardsManager.Instance.SetRewardType(RewardsType.WIN);
        RewardsManager.Instance.SetCurrentLevel(4);
        WorldManager.Instance.SetWorldComplete(true, LevelManager.Instance.GetCurrentLevel() - 1);
        //WorldManager.Instance.SetUnlockStateIndex(LevelManager.Instance.GetCurrentLevel(), true);
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(BackToMissionSelect());
    }

    void Intro()
    {
        FadeScreenManager.Instance.FadeOut();
        StartCoroutine(IntroScene());
    }

    IEnumerator IntroScene()
    {
        DialogueManager.Instance.ShowCanvas();
        //Disable PlayerInput if kaya
        yield return new WaitForSeconds(1.25f);

        DialogueManager.Instance.StartDialogue(DialogueType.TUTORIAL_HUB);
        //play music
        SoundManager.Instance.PlayMusic("HUB");

        yield return new WaitForSeconds(0.5f);
    }

    public void ToMechSelect()
    {
        FadeScreenManager.Instance.FadeIn();
        StartCoroutine(MechSelect());
        FadeScreenManager.Instance.FadeOut();
        if (firstTimeDeployment)
            StartCoroutine(MechSelectText());
    }

    IEnumerator MechSelect()
    {
        yield return new WaitForSeconds(0.75f);

        //Disable Mission Select Scene
        missionSelect.SetActive(false);
        toArmory.SetActive(false);
        CorruptionManager.Instance.DisableCanvas();
        CurrencyManager.Instance.DisableCanvas();

        mechSelect.transform.Find("MainUI").gameObject.SetActive(true);

        //Camera
        virtualCam.LookAt = armory.transform.Find("Target");
        virtualCam.Follow = armory.transform.Find("Target");

        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator MechSelectText()
    {
        DialogueManager.Instance.ShowCanvas();
        yield return new WaitForSeconds(1.25f);
        DialogueManager.Instance.StartDialogue(DialogueType.MECH_DEPLOYMENT);
        yield return new WaitForSeconds(0.5f);
        firstTimeDeployment = false;
    }

    public void Return()
    {
        FadeScreenManager.Instance.FadeIn();
        StartCoroutine(MissionSelect());
        FadeScreenManager.Instance.FadeOut();
    }

    public void ToArmory()
    {
        FadeScreenManager.Instance.FadeIn();
        StartCoroutine(Armory());
        FadeScreenManager.Instance.FadeOut();
        if (firstTimeArmory)
            StartCoroutine(ArmoryText());
    }

    IEnumerator ArmoryText()
    {
        DialogueManager.Instance.ShowCanvas();
        yield return new WaitForSeconds(1.25f);
        DialogueManager.Instance.StartDialogue(DialogueType.ARMORY);
        yield return new WaitForSeconds(0.5f);
        firstTimeArmory = false;
    }

    IEnumerator Armory()
    {
        yield return new WaitForSeconds(1f);

        //Disable Mission Select Scene
        planetLight.SetActive(false);
        missionSelect.SetActive(false);
        toArmory.SetActive(false);
        CorruptionManager.Instance.DisableCanvas();
        CurrencyManager.Instance.EnableCanvas();

        //Enable Armory
        // armory.SetActive(true);
        armory.transform.Find("Content").gameObject.SetActive(true);

        //Cam Settings
        virtualCam.LookAt = armory.transform.Find("Content/Garage/Mech");
        virtualCam.Follow = armory.transform.Find("Content/Garage/Mech");

        pos.TargetOffset.y = 12;
        pos.TargetOffset.z = -20;
        rot.TargetOffset.y = 7;

        //cam.
        cam.GetComponent<HDAdditionalCameraData>().clearColorMode = HDAdditionalCameraData
            .ClearColorMode
            .Color;

        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator MissionSelect()
    {
        yield return new WaitForSeconds(1f);

        //Disable Mission Select Scene
        planetLight.SetActive(true);
        missionSelect.SetActive(true);
        toArmory.SetActive(true);
        CorruptionManager.Instance.EnableCanvas();
        CurrencyManager.Instance.EnableCanvas();

        //Enable Armory
        armory.transform.Find("Content").gameObject.SetActive(false);
        mechSelect.transform.Find("MainUI").gameObject.SetActive(false);

        //Cam Settings
        virtualCam.LookAt = missionSelect.transform.Find("Planet");
        virtualCam.Follow = missionSelect.transform.Find("Planet");

        pos.TargetOffset.y = 0;
        pos.TargetOffset.z = -7;
        rot.TargetOffset.y = 0;

        // cam.clearFlags = CameraClearFlags.Skybox;
        cam.GetComponent<HDAdditionalCameraData>().clearColorMode = HDAdditionalCameraData
            .ClearColorMode
            .Sky;

        yield return new WaitForSeconds(0.5f);
    }

    public void ToMissionSelect()
    {
        FadeScreenManager.Instance.FadeIn();
        StartCoroutine(MissionSelect());
        FadeScreenManager.Instance.FadeOut();
    }

    public void ToLevel()
    {
        FadeScreenManager.Instance.FadeIn();
        StartCoroutine(Level());
        FadeScreenManager.Instance.FadeOut();
        StartCoroutine(LevelText());
    }

    IEnumerator Level()
    {
        yield return new WaitForSeconds(0.5f);

        SoundManager.Instance.StopMusic();
        LevelManager.Instance.LoadLevelIndex(LevelManager.Instance.GetCurrentLevel());

        switch (LevelManager.Instance.GetCurrentLevel())
        {
            case 1:
            case 2:
            case 3:
            case 4:
                SoundManager.Instance.PlayMusic("Forest");
                break;
        }

        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator LevelText()
    {
        DialogueManager.Instance.ShowCanvas();
        yield return new WaitForSeconds(1.25f);
        switch (LevelManager.Instance.GetCurrentLevel())
        {
            case 1:
                DialogueManager.Instance.StartDialogue(DialogueType.FOREST1);
                break;
            case 2:
                DialogueManager.Instance.StartDialogue(DialogueType.FOREST2);
                break;
            case 3:
                DialogueManager.Instance.StartDialogue(DialogueType.FOREST3);
                break;
            case 4:
                DialogueManager.Instance.StartDialogue(DialogueType.FOREST4);
                break;
        }
        yield return new WaitForSeconds(0.5f);
    }
}
