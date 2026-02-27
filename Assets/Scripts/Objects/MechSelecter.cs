using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class MechSelecter : MonoBehaviour
{
    [SerializeField] Transform target;
    Transform cam;
    [SerializeField] float speed;
    void Awake()
    {
        cam = Camera.main.transform;
    }

    void Update()
    {
        RotateMech();
    }

    void RotateMech()
    {
        transform.Rotate(Vector3.up, InputManager.Instance.GetRotateY() * speed);
    }
}