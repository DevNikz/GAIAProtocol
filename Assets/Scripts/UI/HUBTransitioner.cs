using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class HUBTransitioner : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCameraBase virtualCam;
    CinemachineOrbitalFollow pos;
    CinemachineRotationComposer rot;
    [SerializeField] GameObject missionSelect;
    [SerializeField] GameObject armory;
    [SerializeField] GameObject mechSelect;
    [SerializeField] GameObject toArmory;

    Camera cam;

    void Awake()
    {
        cam = Camera.main;
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
    }

    IEnumerator Armory()
    {
        yield return new WaitForSeconds(1f);

        //Disable Mission Select Scene
        missionSelect.SetActive(false);
        toArmory.SetActive(false);
        CorruptionManager.Instance.DisableCanvas();
        CurrencyManager.Instance.DisableCanvas();

        //Enable Armory
        // armory.SetActive(true);
        armory.transform.Find("Content").gameObject.SetActive(true);

        //Cam Settings
        virtualCam.LookAt = armory.transform.Find("Content/Garage/Mech/mesh");
        virtualCam.Follow = armory.transform.Find("Content/Garage/Mech/mesh");

        pos.TargetOffset.y = 21;
        pos.TargetOffset.z = -20;
        rot.TargetOffset.y = 15;

        //cam.
        cam.GetComponent<HDAdditionalCameraData>().clearColorMode = HDAdditionalCameraData.ClearColorMode.Color;

        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator MissionSelect()
    {
        yield return new WaitForSeconds(0.5f);

        //Disable Mission Select Scene
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
    }

    IEnumerator Level()
    {
        yield return new WaitForSeconds(0.5f);

        LevelManager.Instance.LoadLevelIndex(LevelManager.Instance.GetCurrentLevel());

        yield return new WaitForSeconds(0.5f);
    }
}
