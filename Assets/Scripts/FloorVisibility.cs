using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorVisibility : MonoBehaviour
{

    [SerializeField] private bool dynamicFloorPosition;
    [SerializeField] private List<Renderer> ignoreRendererList;

    private Renderer[] rendererArray;
    private int floor;

    private void Awake()
    {
        rendererArray = GetComponentsInChildren<Renderer>(true);
    }

    private void Start()
    {
        floor = LevelGrid.Instance.GetFloor(transform.position);

        if (floor == 0 && !dynamicFloorPosition)
        {
            Destroy(this);
        }
    }

    private void Update() 
    {
        if (dynamicFloorPosition)
        {
            floor = LevelGrid.Instance.GetFloor(transform.position);
        }

        float cameraHeight = CameraController.Instance.GetCameraHeight();

        float floorHeightOffset = 2f;
        bool showObject = cameraHeight > LevelGrid.FLOOR_HEIGHT * floor + floorHeightOffset;

        if (showObject || floor == 0)
        {
            Show();
        } else
        {
            Hide();
        }
    }

    private void Show()
    {
        foreach (Renderer renderer in rendererArray)
        {
            if (ignoreRendererList.Contains(renderer)) continue;
            renderer.enabled = true;
        }
    }

    private void Hide()
    {
        foreach (Renderer renderer in rendererArray)
        {
            if (ignoreRendererList.Contains(renderer)) continue;
            renderer.enabled = false;
        }
    }

}