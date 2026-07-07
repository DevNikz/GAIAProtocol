using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ObjectiveScreenUI : MonoBehaviour
{
    [SerializeField]
    private GameObject objectiveChecklistRef;

    [SerializeField]
    private Transform container;

    [SerializeField]
    private List<ObjUI> objectiveList;

    [SerializeField]
    private GameObject UICanvas;

    [SerializeField]
    private int objectiveNum;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ClearObjectives();
        switch (scene.buildIndex)
        {
            case 1:
            case 2:
            case 3:
                //UICanvas.SetActive(true);
                SetupObjectives();
                break;
            default:
                UICanvas.SetActive(false);
                break;
        }
    }

    void LateUpdate()
    {
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 1:
            case 2:
            case 3:
                if (ObjectiveManager.Instance.IsInCutscene())
                    UICanvas.SetActive(false);
                else
                    UICanvas.SetActive(true);
                break;
            default:
                break;
        }
    }

    void SetupObjectives()
    {
        /*
        objectiveNum = ObjectiveManager.Instance.GetObjectiveCount();

        for (int i = 0; i < objectiveNum; i++)
        {
            GameObject obj = Instantiate(objectiveChecklistRef, container);

            obj.GetComponent<ObjUI>()
                .SetUI(ObjectiveManager.Instance.objectivesList[i].description);

            objectiveList.Add(obj.GetComponent<ObjUI>());
        }
        */
    }

    void ClearObjectives()
    {
        objectiveNum = 0;
        objectiveList = new List<ObjUI>();
        if (objectiveList.Count > 0)
            objectiveList.Clear();
    }

    public void SetToggleUI(int index)
    {
        objectiveList[index].SetBool(true);
    }
}
