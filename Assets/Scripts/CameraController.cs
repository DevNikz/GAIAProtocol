using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
using System;

public class CameraController : MonoBehaviour
{

    private const float MIN_FOLLOW_Y_OFFSET = 2f;
    private const float MAX_FOLLOW_Y_OFFSET = 15f;
    [SerializeField, Range(0.1f, 100f)] private float moveSpeed = 10f;
    [SerializeField, Range(0.1f, 100f)] private float rotationSpeed = 100f;
    [SerializeField, Range(0.1f, 100f)] private float zoomSpeed = 2f;

    public static CameraController Instance { get; private set; }

    [SerializeField] private CinemachineVirtualCameraBase cinemachineVirtualCamera;
    private CinemachineFollow cinemachineFollow;
    private Vector3 targetFollowOffset;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        cinemachineFollow = cinemachineVirtualCamera.GetComponent<CinemachineFollow>();
        targetFollowOffset = cinemachineFollow.FollowOffset;
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleZoom();
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

}