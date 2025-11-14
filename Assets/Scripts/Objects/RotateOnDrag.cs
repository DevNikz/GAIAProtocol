using UnityEngine;

public class RotateOnDrag : MonoBehaviour
{
    [SerializeField, Range(0.1f, 10f)] private float _speed;
    [SerializeField] private bool _inverted;
    private Vector3 _mouseRef;
    private bool _rotateAllowed;
    

    void OnMouseDrag()
    {
        float xAxis = Input.GetAxis("Mouse X") * _speed;
        float yAxis = Input.GetAxis("Mouse Y") * _speed;

        transform.Rotate(Vector3.down, xAxis);
        transform.Rotate(Vector3.right, yAxis);
    }

    // void Update()
    // {
    //     if(!Input.GetMouseButton(0)) return;
    //     else _mouseRef = Input.mousePosition;


    //     Vector2 mouseDelta = InputManager.Instance.GetMouseScreenPosition();
    //     mouseDelta *= _speed * Time.deltaTime;
    //     transform.Rotate(Vector3.up * (_inverted ? 1 : -1), mouseDelta.x, Space.World);
    //     transform.Rotate(Vector3.right * (_inverted ? -1 : 1), mouseDelta.y, Space.World);
    // }

    // void OnMouseDown()
    // {
    //     _rotateAllowed = true;
    //     _mouseRef = Input.mousePosition;
    // }

    // void OnMouseUp()
    // {
    //     _rotateAllowed = false;
    // }
}
