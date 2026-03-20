using UnityEngine;

public class RTSCameraBounds : MonoBehaviour
{
    [Header("Map Bounds (World Space)")]
    public float minX = -2f;
    public float maxX =  40f;
    public float minZ = -2f;
    public float maxZ =  60f;

    [Header("Boundary Offset")]
    [Tooltip("How far the camera stops before reaching the bound wall")]
    public float stopOffset = 10f;

    private float clampMinX, clampMaxX, clampMinZ, clampMaxZ;

    void Start()
    {
        RecalculateBounds();
    }

    // Call this if bounds change at runtime
    public void RecalculateBounds()
    {
        clampMinX = minX + stopOffset;
        clampMaxX = maxX - stopOffset;
        clampMinZ = minZ + stopOffset;
        clampMaxZ = maxZ - stopOffset;
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