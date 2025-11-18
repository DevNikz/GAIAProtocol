using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance { get; private set; }

    public List<bool> worldAreaCompletion;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        worldAreaCompletion = new List<bool> ( new bool[9] );
    }

    public bool GetWorldComplete(int index)
    {
        return worldAreaCompletion[index];
    }

    public void SetWorldComplete(bool value, int index)
    {
        worldAreaCompletion[index] = value;
    }
}