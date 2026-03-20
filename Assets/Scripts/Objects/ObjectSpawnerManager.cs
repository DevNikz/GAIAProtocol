using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectSpawnerManager : MonoBehaviour
{
    public static ObjectSpawnerManager Instance;

    [SerializeField] private List<GameObject> objectList;
    [SerializeField] private List<GameObject> objectSpawnPoints;
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
        hasSpawnedObjects = false;
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
                hasSpawnedObjects = false;
                ClearObjects();
                ClearSpawnPoints();
                break;
            case 1:
                if(!hasSpawnedObjects) 
                {
                    StartCoroutine(InitSpawner());
                    break;
                }
                break;
        }
    }

    // void OnDisable()
    // {
    //     SceneManager.sceneLoaded -= OnSceneLoaded;
    //     hasSpawnedObjects = false;
    // }

    IEnumerator InitSpawner()
    {
        yield return new WaitForSeconds(0.001f);

        ClearObjects();

        yield return new WaitForSeconds(0.001f);
        ClearSpawnPoints();

        yield return new WaitForSeconds(0.001f);
        SetSpawnPoints();

        yield return new WaitForSeconds(0.001f);
        SpawnRandomObjects();
    }

    void SetSpawnPoints()
    {
        GameObject[] points = GameObject.FindGameObjectsWithTag("Spawnpoints");
        foreach (GameObject point in points)
        {
            objectSpawnPoints.Add(point);
        }

        Debug.Log($"Found {objectSpawnPoints.Count} spawn points");
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
        //ClearSpawnPoints();
    }

    void ClearSpawnPoints()
    {
        objectSpawnPoints = new List<GameObject>(); // already empty
        if(objectSpawnPoints.Count > 0) objectSpawnPoints.Clear(); // never true
    }
    
    void SpawnRandomObjects()
    {
        //foreach (GameObject points in objectSpawnPoints)
        for(int i = 0; i < objectSpawnPoints.Count; i++)
        {
            int objIndex = Random.Range(0, objectList.Count);

            GameObject obj = Instantiate(objectList[objIndex], objectSpawnPoints[i].transform);

            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            obj.transform.localScale = new Vector3(objectScale, objectScale, objectScale);
            //obj.isStatic = true;

            //int layerNum = LayerMask.NameToLayer("Obstacles");
            //obj.layer = layerNum;
        }

        Debug.Log("Has Spawned Objects");
        hasSpawnedObjects = true;
    }
}
