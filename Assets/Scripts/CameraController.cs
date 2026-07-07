using System;
using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private const float MIN_FOLLOW_Y_OFFSET = 5f;
    private const float MAX_FOLLOW_Y_OFFSET = 15f;

    [SerializeField, Range(0.1f, 100f)]
    private float moveSpeed = 10f;

    [SerializeField, Range(0.1f, 100f)]
    private float rotationSpeed = 100f;

    [SerializeField, Range(0.1f, 100f)]
    private float zoomSpeed = 2f;

    [SerializeField, Range(0.01f, 5f)]
    private float dragPanSensitivity = 1f;

    [SerializeField, Range(1f, 100f)]
    private float dragDeadZone = 10f;

    [SerializeField, Range(50f, 1000f)]
    private float dragMaxDistance = 250f;

    [SerializeField]
    bool isDragging;

    public static CameraController Instance { get; private set; }

    [SerializeField]
    private CinemachineVirtualCameraBase cinemachineVirtualCamera;

    [SerializeField]
    private Camera mainCamera;

    public CinemachineVirtualCameraBase GetCam()
    {
        return cinemachineVirtualCamera;
    }

    private CinemachineFollow cinemachineFollow;
    private Vector3 targetFollowOffset;

    [Header("Map Bounds (World Space)")]
    public float minX = -2f;
    public float maxX = 40f;
    public float minZ = -2f;
    public float maxZ = 60f;

    [Header("Boundary Offset")]
    [Tooltip("How far the camera stops before reaching the bound wall")]
    public float stopOffset = 10f;

    private float clampMinX,
        clampMaxX,
        clampMinZ,
        clampMaxZ;

    private Vector3 dragOrigin;
    Vector3 origin;
    Vector3 difference;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        cinemachineFollow = cinemachineVirtualCamera.GetComponent<CinemachineFollow>();
        targetFollowOffset = cinemachineFollow.FollowOffset;
        mainCamera = Camera.main;

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
        HandleDrag();
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

        targetFollowOffset.y = Mathf.Clamp(
            targetFollowOffset.y,
            MIN_FOLLOW_Y_OFFSET,
            MAX_FOLLOW_Y_OFFSET
        );

        cinemachineFollow.FollowOffset = Vector3.Lerp(
            cinemachineFollow.FollowOffset,
            targetFollowOffset,
            Time.deltaTime * zoomSpeed
        );
    }

    public float GetCameraHeight()
    {
        return targetFollowOffset.y;
    }

    private void HandleDrag()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isDragging = true;
            dragOrigin = Input.mousePosition;
            return;
        }

        if (Input.GetMouseButtonUp(1))
        {
            isDragging = false;
        }

        if (!isDragging)
        {
            return;
        }

        difference = Input.mousePosition - dragOrigin;

        if (difference.sqrMagnitude < dragDeadZone * dragDeadZone)
        {
            return;
        }

        Vector2 direction = new Vector2(difference.x, difference.y).normalized;
        float speedScale = Mathf.Clamp01((difference.magnitude - dragDeadZone) / dragMaxDistance);

        Vector3 right = transform.right;
        right.y = 0f;
        right.Normalize();

        Vector3 forward = transform.forward;
        forward.y = 0f;
        forward.Normalize();

        origin = right * direction.x + forward * direction.y;
        transform.position += origin * moveSpeed * speedScale * dragPanSensitivity * Time.deltaTime;

        /*
        Vector3 mouseScreenPos = Input.mousePosition;
        difference = mouseScreenPos - dragOrigin;
        dragOrigin = mouseScreenPos;

        if (difference == Vector3.zero)
        {
            return;
        }

        Vector3 right = transform.right;
        right.y = 0f;
        right.Normalize();

        Vector3 forward = transform.forward;
        forward.y = 0f;
        forward.Normalize();

        float scale = targetFollowOffset.y * dragPanSensitivity * 0.002f;

        origin = (right * -difference.x + forward * -difference.y) * scale;
        transform.position += origin;
        */
    }

    private Vector3 GetGroundPoint(Vector3 screenPosition)
    {
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0f, transform.position.y, 0f));
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);

        if (groundPlane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }

        return dragOrigin;
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
