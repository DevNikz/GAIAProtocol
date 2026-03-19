using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class HUBTransitioner : MonoBehaviour
{
    public static HUBTransitioner Instance;

    [SerializeField] CinemachineVirtualCameraBase virtualCam;
    CinemachineOrbitalFollow pos;
    CinemachineRotationComposer rot;
    [SerializeField] GameObject missionSelect;
    [SerializeField] GameObject armory;
    [SerializeField] GameObject mechSelect;
    [SerializeField] GameObject toArmory;
    [SerializeField] GameObject planetLight;

    Camera cam;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        cam = Camera.main;
        planetLight = GameObject.FindGameObjectWithTag("PlanetLight");
        missionSelect = GameObject.FindGameObjectWithTag("MissionSelecter");

        if(GameObject.FindGameObjectWithTag("Armory") != null) {
            armory = GameObject.FindGameObjectWithTag("Armory");
            armory.transform.Find("Content").gameObject.SetActive(false);
        }

        if(GameObject.FindGameObjectWithTag("MechSelect") != null)
        {
            mechSelect = GameObject.FindGameObjectWithTag("MechSelect");
            mechSelect.transform.Find("MainUI").gameObject.SetActive(false);
        }

        if(GameObject.FindGameObjectWithTag("ToArmory") != null)
        {
            toArmory = GameObject.FindGameObjectWithTag("ToArmory");
        }

        if(GameObject.FindGameObjectWithTag("VirtualCam") != null) {
            virtualCam = FindAnyObjectByType<CinemachineVirtualCameraBase>();
            pos = virtualCam.GetComponent<CinemachineOrbitalFollow>();
            rot = virtualCam.GetComponent<CinemachineRotationComposer>();
        }
    }

    void Start()
    {
        Intro();
    }

    public void ExtractForest1()
    {
        FadeScreenManager.Instance.FadeIn();
        StartCoroutine(BackToMissionSelect());
        FadeScreenManager.Instance.FadeOut();
    }

    IEnumerator BackToMissionSelect()
    {
        yield return new WaitForSeconds(1f);

        CurrencyManager.Instance.SetResearchPoints(CurrencyManager.Instance.GetPromptedPoints());
        WorldManager.Instance.SetWorldComplete(true, LevelManager.Instance.GetCurrentLevel() - 1);
        CorruptionManager.Instance.SetCorruptionByIndex(0, 0.52f);
        LevelManager.Instance.LoadLevelIndex(0); //HUB

        //View Rewards Result Screen Later On

        yield return new WaitForSeconds(0.5f);
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
        StartCoroutine(ArmoryText());
    }

    IEnumerator ArmoryText()
    {
        DialogueManager.Instance.ShowCanvas();
        //InputManager.Instance.DisableMechRotate();
        //Disable PlayerInput if kaya
        yield return new WaitForSeconds(1.25f);
        DialogueManager.Instance.StartDialogue(DialogueType.TUTORIAL_HUB);
        yield return new WaitForSeconds(0.5f);
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
        cam.GetComponent<HDAdditionalCameraData>().clearColorMode = HDAdditionalCameraData.ClearColorMode.Color;

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
        cam.GetComponent<HDAdditionalCameraData>().clearColorMode = HDAdditionalCameraData.ClearColorMode.Sky;

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
        
        switch(LevelManager.Instance.GetCurrentLevel())
        {
            case 1:
            case 2:
            case 3:
            SoundManager.Instance.PlayMusic("Forest");
            break;
        }
    
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator LevelText()
    {
        DialogueManager.Instance.ShowCanvas();
        yield return new WaitForSeconds(1.25f);
        switch(LevelManager.Instance.GetCurrentLevel())
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
        }
        yield return new WaitForSeconds(0.5f);
    }
}
