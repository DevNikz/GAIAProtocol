using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectSpawnerManager : MonoBehaviour
{
    public static ObjectSpawnerManager Instance;

    [SerializeReference] private List<GameObject> objectList;
    [SerializeReference] private List<GameObject> objectSpawnPoints;
    private bool hasSpawnedObjects;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch(scene.buildIndex)
        {
            case 1:
                if(!hasSpawnedObjects) 
                {
                    ClearSpawnPoints();
                    SetSpawnPoints();
                    SpawnRandomObjects();
                    break;
                }
                else break;
            default:
                ClearSpawnPoints();
                break;
        }
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        hasSpawnedObjects = false;
    }

    void SetSpawnPoints()
    {
        foreach (Transform child in transform.GetComponentsInChildren<Transform>()) {
            if(child.name != this.name)
            {
                //Debug.Log($"Added {child.name}");
                objectSpawnPoints.Add(child.gameObject);
            } 
        }
    }

    void ClearSpawnPoints()
    {
        objectSpawnPoints = new List<GameObject>();
        if(objectSpawnPoints.Count > 0) objectSpawnPoints.Clear();
    }
    
    void SpawnRandomObjects()
    {
        //foreach (GameObject points in objectSpawnPoints)
        for(int i = 0; i < objectSpawnPoints.Count; i++)
        {
            int objIndex = Random.Range(0, objectList.Count);
            //Debug.Log($"{objectList[objIndex].name} has spawned in {points.name}");

            GameObject obj = Instantiate(objectList[objIndex], objectSpawnPoints[i].transform);

            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            obj.transform.localScale = new Vector3(2,2,2);
            obj.isStatic = true;

            int layerNum = LayerMask.NameToLayer("Obstacles");
            obj.layer = layerNum;
        }

        hasSpawnedObjects = true;
    }
}
