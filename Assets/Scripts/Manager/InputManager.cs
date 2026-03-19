#define USE_NEW_INPUT_SYSTEM
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{

    public static InputManager Instance { get; private set; }

    private PlayerInputActions playerInputActions;

    [SerializeField] bool disableKeyInputs;
    [SerializeField] bool disableMechRotate;
    [SerializeField] bool disableDebug;
    [SerializeField] bool disableLevelCamera;
    public bool AreLegacyInputsDisabled() { return disableKeyInputs; }
    public void EnableLegacyInputs() { disableKeyInputs = false; }
    public void DisableLegacyInputs() { disableKeyInputs = true; }
    public void SetLegacyInputs(bool value) { disableKeyInputs = value; }
    public void EnableLevelCamera() { disableLevelCamera = false; }
    public void DisableLevelCamera() { disableLevelCamera = true; }
    public void SetLevelCam(bool value) { disableLevelCamera = value; }
    public void EnableDebug() { disableDebug = false; }
    public void DisableDebug() { disableDebug = true; } 
    public void SetDebug(bool value) { disableDebug = value; }
    public void EnableMechRotate() { disableMechRotate = false;}
    public void DisableMechRotate() { disableMechRotate = true; }
    public void SetMechRotate(bool value) { disableMechRotate = value; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            playerInputActions = new PlayerInputActions();
            playerInputActions.Player.Enable();
        }
        else Destroy(gameObject);        
    }

    public Vector2 GetMouseScreenPosition()
    {
#if USE_NEW_INPUT_SYSTEM
        return Mouse.current.position.ReadValue();
#else
        return Input.mousePosition;
#endif
    }

    public Vector3 GetMouseScreenPositionV3()
    {
#if USE_NEW_INPUT_SYSTEM
        return Mouse.current.position.ReadValue();
#else
        return Input.mousePosition;
#endif
    }

    public bool IsMouseButtonDownThisFrame()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.Click.WasPressedThisFrame();
#else
        return Input.GetMouseButtonDown(0);
#endif
    }

    public bool IsMouseHeldDown()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.Hold.WasPerformedThisFrame();
#else
        return Input.GetMouseButtonDown(0);
#endif
    }

    public Vector2 GetCameraMoveVector()
    {
        if(disableLevelCamera) return Vector2.zero;

#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.CameraMovement.ReadValue<Vector2>();
#else
        Vector2 inputMoveDir = new Vector2(0, 0);

        if (Input.GetKey(KeyCode.W))
        {
            inputMoveDir.y = +1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            inputMoveDir.y = -1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            inputMoveDir.x = -1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputMoveDir.x = +1f;
        }

        return inputMoveDir;
#endif
    }

    public float GetCameraRotateAmount()
    {
        if(disableLevelCamera) return 0;

#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.CameraRotate.ReadValue<float>();
#else
        float rotateAmount = 0f;

        if (Input.GetKey(KeyCode.Q))
        {
            rotateAmount = +1f;
        }
        if (Input.GetKey(KeyCode.E))
        {
            rotateAmount = -1f;
        }

        return rotateAmount;
#endif
    }

    public float GetRotateY()
    {
        if(disableMechRotate) return 0;

#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.RotateY.ReadValue<float>();
#else
        float rotateAmount = 0f;

        if (Input.GetKey(KeyCode.D))
        {
            rotateAmount = -1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            rotateAmount = +1f;
        }

        return rotateAmount;
#endif
    }

    public float GetCameraZoomAmount()
    {
        if(disableLevelCamera) return 0;

#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.CameraZoom.ReadValue<float>();
#else
        float zoomAmount = 0f;

        if (Input.mouseScrollDelta.y > 0)
        {
            zoomAmount = -1f;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            zoomAmount = +1f;
        }

        return zoomAmount;
#endif
    }

    public bool GetDebugButton()
    {
        if(disableDebug) return false;

#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.ToggleDebug.WasPressedThisFrame();
#else
        return Input.GetKey(KeyCode.BackQuote);
#endif
     }

    public bool GetReturnButton()
    {
        if(disableDebug) return false;

#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.Return.WasPressedThisFrame();
#else
        return Input.GetKey(KeyCode.return);
#endif
     }
}
