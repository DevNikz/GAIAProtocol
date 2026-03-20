using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
using System;

public class CameraController : MonoBehaviour
{

    private const float MIN_FOLLOW_Y_OFFSET = 5f;
    private const float MAX_FOLLOW_Y_OFFSET = 15f;
    [SerializeField, Range(0.1f, 100f)] private float moveSpeed = 10f;
    [SerializeField, Range(0.1f, 100f)] private float rotationSpeed = 100f;
    [SerializeField, Range(0.1f, 100f)] private float zoomSpeed = 2f;

    public static CameraController Instance { get; private set; }

    [SerializeField] private CinemachineVirtualCameraBase cinemachineVirtualCamera;
    public CinemachineVirtualCameraBase GetCam() { return cinemachineVirtualCamera; }
    private CinemachineFollow cinemachineFollow;
    private Vector3 targetFollowOffset;

    [Header("Map Bounds (World Space)")]
    public float minX = -2f;
    public float maxX =  40f;
    public float minZ = -2f;
    public float maxZ =  60f;

    [Header("Boundary Offset")]
    [Tooltip("How far the camera stops before reaching the bound wall")]
    public float stopOffset = 10f;

    private float clampMinX, clampMaxX, clampMinZ, clampMaxZ;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        cinemachineFollow = cinemachineVirtualCamera.GetComponent<CinemachineFollow>();
        targetFollowOffset = cinemachineFollow.FollowOffset;

        RecalculateBounds();
    }

    public void RecalculateBounds()
    {
        clampMinX = minX + stopOffset;
        clampMaxX = maxX - stopOffset;
        clampMinZ = minZ + stopOffset;
        clampMaxZ = maxZ - stopOffset;
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleZoom();
    }

    void LateUpdate()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, clampMinX, clampMaxX);
        pos.z = Mathf.Clamp(pos.z, clampMinZ, clampMaxZ);
        transform.position = pos;
    }

    void OnValidate()
    {
        RecalculateBounds();
    }

    private void HandleMovement()
    {
        Vector2 inputMoveDir = InputManager.Instance.GetCameraMoveVector();

        Vector3 moveVector = transform.forward * inputMoveDir.y + transform.right * inputMoveDir.x;
        transform.position += moveVector * moveSpeed * Time.deltaTime;
    }

    private void HandleRotation()
    {
        Vector3 rotationVector = new Vector3(0, 0, 0);

        rotationVector.y = InputManager.Instance.GetCameraRotateAmount();

        transform.eulerAngles += rotationVector * rotationSpeed * Time.deltaTime;
    }

    private void HandleZoom()
    {
        float zoomIncreaseAmount = 1f;
        targetFollowOffset.y += InputManager.Instance.GetCameraZoomAmount() * zoomIncreaseAmount;

        targetFollowOffset.y = Mathf.Clamp(targetFollowOffset.y, MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFSET);

        cinemachineFollow.FollowOffset =
            Vector3.Lerp(cinemachineFollow.FollowOffset, targetFollowOffset, Time.deltaTime * zoomSpeed);
    }
    

    public float GetCameraHeight()
    {
        return targetFollowOffset.y;
    }

    #if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        // Draw the raw map boundary (red)
        Gizmos.color = Color.red;
        DrawBoundsGizmo(minX, maxX, minZ, maxZ);

        // Draw the camera stop boundary (yellow)
        Gizmos.color = Color.yellow;
        DrawBoundsGizmo(clampMinX, clampMaxX, clampMinZ, clampMaxZ);
    }

    void DrawBoundsGizmo(float x0, float x1, float z0, float z1)
    {
        float y = transform.position.y;
        Vector3 a = new Vector3(x0, y, z0);
        Vector3 b = new Vector3(x1, y, z0);
        Vector3 c = new Vector3(x1, y, z1);
        Vector3 d = new Vector3(x0, y, z1);

        Gizmos.DrawLine(a, b);
        Gizmos.DrawLine(b, c);
        Gizmos.DrawLine(c, d);
        Gizmos.DrawLine(d, a);
    }
#endif

}