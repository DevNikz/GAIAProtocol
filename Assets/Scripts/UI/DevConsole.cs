using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DevConsole : MonoBehaviour
{
    public static DevConsole Instance { get; private set; }
    
    bool showConsole;
    string input;

    public static DevCommand RESTART;
    public static DevCommand RESTART_HUB;
    public List<object> commandList;

    [SerializeField] int fontSize = 32;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        RESTART = new DevCommand("restart", "Restart Current Level.", "restart", () =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            GridSystemVisual.Instance.SetVisuals(false);
            TurnSystem.Instance.ResetSys();
            ObjectiveManager.Instance.ResetSys();
            ObjectSpawnerManager.Instance.SetHasSpawned(false);
        });

        RESTART_HUB = new DevCommand("restartHUB", "Restart HUB", "restartHUB", () =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        });

        commandList = new List<object>
        {
            RESTART
        };
    }

    void Update()
    {
        if(InputManager.Instance.GetDebugButton()) OnToggleDebug();
        if(InputManager.Instance.GetReturnButton()) OnReturn();
    }

    void OnToggleDebug()
    {
        showConsole = !showConsole;
        InputManager.Instance.SetLevelCam(showConsole);
        InputManager.Instance.SetMechRotate(showConsole);
        InputManager.Instance.SetLegacyInputs(showConsole);
        //InputManager.Instance.SetPlayerInput(showConsole);
    }

    void OnReturn()
    {
        if(showConsole)
        {
            HandleInput();
            input = "";
        }
    }

    void HandleInput()
    {
        for(int i = 0; i < commandList.Count; i++)
        {
            DevCommandBase commandBase = commandList[i] as DevCommandBase;
            if(input.Contains(commandBase.commandId))
            {
                if(commandList[i] as DevCommand != null)
                {
                    (commandList[i] as DevCommand).Invoke();
                }
            }
        }
    }

    private void OnGUI()
    {
        if(!showConsole) return;

        float y = 0f;

        GUI.Box(new Rect(0, y, Screen.width, 30), "");
        GUI.backgroundColor = new Color(0, 0, 0, 0);
        GUIStyle myFont = new GUIStyle();
        myFont.fontSize = fontSize;
        myFont.normal.textColor = Color.white;
        input = GUI.TextField(new Rect(10f, y + 5f, Screen.width - 20f, 50f), input);
    }
}
