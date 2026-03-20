using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectSpawnerManager : MonoBehaviour
{
    public static ObjectSpawnerManager Instance;

    [SerializeReference] private List<GameObject> objectList;
    [SerializeReference] private List<GameObject> objectSpawnPoints;
    private bool hasSpawnedObjects;

    public bool HasSpawnedObjects() { return hasSpawnedObjects; }
    public void SetHasSpawned(bool value) { hasSpawnedObjects = value; } 

    [SerializeField] float objectScale = .75f;

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
            default:
            case 0:
                ClearObjects();
                break;
            case 1:
                if(!hasSpawnedObjects) 
                {
                    ClearObjects();
                    ClearSpawnPoints();
                    SetSpawnPoints();
                    SpawnRandomObjects();
                    break;
                }
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

    void ClearObjects()
    {
        for(int i = 0; i < objectSpawnPoints.Count; i++)
        {
            foreach (Transform objectsSpawned in objectSpawnPoints[i].transform)
            {
                Destroy(objectsSpawned.gameObject);
            }
        }
        ClearSpawnPoints();
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
            obj.transform.localScale = new Vector3(objectScale, objectScale, objectScale);
            obj.isStatic = true;

            int layerNum = LayerMask.NameToLayer("Obstacles");
            obj.layer = layerNum;
        }

        hasSpawnedObjects = true;
    }
}
