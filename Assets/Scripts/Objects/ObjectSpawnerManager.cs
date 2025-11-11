using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectSpawnerManager : MonoBehaviour
{
    public static ObjectSpawnerManager Instance;

    [SerializeReference] private List<GameObject> objectList;
    [SerializeReference] private List<GameObject> objectSpawnPoints;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;

        // for (int i = 0; i < transform.childCount; i++)
        // {
        //     Transform childT = transform.GetChild(i);
        //     GameObject childObj = childT.gameObject;
        //     objectSpawnPoints.Add(childObj);
        // }
        // SpawnRandomObjects();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform childT = transform.GetChild(i);
            GameObject childObj = childT.gameObject;
            objectSpawnPoints.Add(childObj);
        }

        SpawnRandomObjects();
    }
    
    void SpawnRandomObjects()
    {
        for (int i = 0; i < objectSpawnPoints.Count; i++)
        {
            int objIndex = Random.Range(0, objectList.Count);
            // int spawnPoint = Random.Range(0, objectSpawnPoints.Count - 1);
            GameObject obj = Instantiate(objectList[objIndex], objectSpawnPoints[i].transform.position, Quaternion.identity);

            //Params
            //float randomScale = Random.Range(3.5f, 4f);
            //obj.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
            obj.transform.localRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

            int layerNum = LayerMask.NameToLayer("Obstacles");
            obj.layer = layerNum;
        }
    }
}
