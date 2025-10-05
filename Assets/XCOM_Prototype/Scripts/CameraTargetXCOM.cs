// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using CodeMonkey.Utils;
// using Cinemachine;

// public class CameraTargetXCOM : MonoBehaviour {

//     private const float MIN_TARGET_FOLLOW_Y = 2f;
//     private const float MAX_TARGET_FOLLOW_Y = 12f;



//     [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;

//     private CinemachineTransposer cinemachineTransposer;
//     private float targetFollowY = 7f;

//     private void Awake() {
//         cinemachineTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
//     }

//     private void Update() {
//         // Handle Movement
//         float moveX = 0f;
//         float moveZ = 0f;

//         if (Input.GetKey(KeyCode.W)) {
//             moveZ = +1f;
//         }
//         if (Input.GetKey(KeyCode.S)) {
//             moveZ = -1f;
//         }
//         if (Input.GetKey(KeyCode.A)) {
//             moveX = -1f;
//         }
//         if (Input.GetKey(KeyCode.D)) {
//             moveX = +1f;
//         }

//         Vector3 moveDir = new Vector3(moveX, 0, moveZ).normalized;

//         Vector3 forward = transform.forward;
//         forward.y = 0;

//         float moveSpeed = 10f;
//         transform.position += (forward * moveZ + transform.right * moveX) * moveSpeed * Time.deltaTime;


//         // Handle Rotation
//         if (Input.GetKey(KeyCode.Q)) {
//             transform.eulerAngles += new Vector3(0, +75, 0) * Time.deltaTime;
//         }
//         if (Input.GetKey(KeyCode.E)) {
//             transform.eulerAngles += new Vector3(0, -75, 0) * Time.deltaTime;
//         }

//         // Handle Zoom
//         if (Input.mouseScrollDelta.y < 0) {
//             targetFollowY = Mathf.Clamp(targetFollowY + 1f, MIN_TARGET_FOLLOW_Y, MAX_TARGET_FOLLOW_Y);
//         }
//         if (Input.mouseScrollDelta.y > 0) {
//             targetFollowY = Mathf.Clamp(targetFollowY - 1f, MIN_TARGET_FOLLOW_Y, MAX_TARGET_FOLLOW_Y);
//         }

//         Vector3 newFollowOffset = cinemachineTransposer.m_FollowOffset;
//         newFollowOffset.y = Mathf.Lerp(newFollowOffset.y, targetFollowY, Time.deltaTime * 10f);
//         cinemachineTransposer.m_FollowOffset = newFollowOffset;
//     }

// }
