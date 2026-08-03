using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;

[RequireComponent(typeof(Image))]
public class HUDMarkerTargetUI : MonoBehaviour
{
    [SerializeField]
    private Image LinkedImage;

    private Image markerImage;

    [SerializeField]
    private HUDMarkerInWorldTarget LinkedTarget;
    private ObjectiveBase objective;
    private RectTransform HUDMarkerRect;

    [SerializeField]
    private int LinkedIndex; //For objective
    private Camera cam;

    private float CachedFOV_Horizontal = -1f;
    private float CachedFOV_Vertical = -1;
    private float CachedAspectRatio = -1f;

    public void Bind(
        HUDMarkerInWorldTarget _Target,
        Sprite _Image,
        ObjectiveBase _Objective,
        int _Index,
        Color _Color
    )
    {
        LinkedTarget = _Target;
        LinkedImage.sprite = _Image;
        objective = _Objective;
        LinkedIndex = _Index;
        LinkedImage.color = _Color;
    }

    void Awake()
    {
        markerImage = GetComponent<Image>();
    }

    void Start()
    {
        HUDMarkerRect = (RectTransform)transform.parent;
        cam = Camera.main;
        //cam = ObjectTransManager.Instance.GetMainCamera();

        // if (!isObjective)
        // {
        //     markerImage.enabled = false;
        //     LinkedImage.enabled = false;
        // }
    }

    public void UpdatePosition()
    {
        if (ShouldDestroyMarker())
        {
            Destroy(gameObject);
            return;
        }

        var viewportPos = cam.WorldToViewportPoint(LinkedTarget.transform.position);

        if (
            viewportPos.x >= 0f
            && viewportPos.x <= 1
            && viewportPos.y >= 0f
            && viewportPos.y <= 1
            && viewportPos.z > 0
        )
        {
            OnScreenRepositionMarker();
        }
        else
        {
            OffScreenRepositionMarker();
        }
    }

    bool ShouldDestroyMarker()
    {
        if (LinkedTarget == null || !LinkedTarget.isActiveAndEnabled)
            return true;

        if (objective == null)
            return true;

        if (objective.IsComplete())
            return true;

        if (objective is ObjectiveCounterTarget counterTarget && counterTarget.HasBeenCollected)
            return true;

        if (objective is ObjectiveInteractFill && ObjectiveManager.Instance != null)
            return ObjectiveManager.Instance.GetComplete(LinkedIndex);

        return false;
    }

    void Update()
    {
        UpdatePosition();
    }

    void OnScreenRepositionMarker()
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(LinkedTarget.transform.position);

        float markerX = ((screenPosition.x / Screen.width) - 0.5f) * HUDMarkerRect.rect.width;
        float markerY = ((screenPosition.y / Screen.height) - 0.5f) * HUDMarkerRect.rect.height;

        ((RectTransform)transform).anchoredPosition = new Vector2(markerX, markerY);
    }

    void OffScreenRepositionMarker()
    {
        Vector3 vecToTarget = (
            LinkedTarget.transform.position - Camera.main.transform.position
        ).normalized;

        // refresh cached data if changed
        if (
            Camera.main.aspect != CachedAspectRatio
            || Camera.main.fieldOfView != CachedFOV_Vertical
        )
        {
            CachedFOV_Vertical = Camera.main.fieldOfView;
            CachedAspectRatio = Camera.main.aspect;

            CachedFOV_Horizontal = Camera.VerticalToHorizontalFieldOfView(
                CachedFOV_Vertical,
                CachedAspectRatio
            );
        }

        // calculate normalised angles to target
        float normalisedX =
            Mathf.Asin(Vector3.Dot(vecToTarget, Camera.main.transform.right))
            * Mathf.Rad2Deg
            / (CachedFOV_Horizontal * 0.5f);
        float normalisedY =
            Mathf.Asin(Vector3.Dot(vecToTarget, Camera.main.transform.up))
            * Mathf.Rad2Deg
            / (CachedFOV_Vertical * 0.5f);

        // clamp to a 0 to 1 range
        normalisedX = Mathf.Clamp01(0.5f * (normalisedX + 1f));
        normalisedY = Mathf.Clamp01(0.5f * (normalisedY + 1f));

        // find the closest edge of the screen
        Vector2 distanceToTopRight = new Vector2(1f - normalisedX, 1f - normalisedY);
        Vector2 distanceToBottomLeft = new Vector2(normalisedX, normalisedY);
        float smallestX = Mathf.Min(distanceToBottomLeft.x, distanceToTopRight.x);
        float smallestY = Mathf.Min(distanceToBottomLeft.y, distanceToTopRight.y);
        float smallestDistance = Mathf.Min(smallestX, smallestY);

        // clamp to edge of screen
        if (smallestDistance == distanceToTopRight.x)
            normalisedX = 1f;
        else if (smallestDistance == distanceToBottomLeft.x)
            normalisedX = 0f;
        else if (smallestDistance == distanceToTopRight.y)
            normalisedY = 1f;
        else if (smallestDistance == distanceToBottomLeft.y)
            normalisedY = 0f;

        // position the marker
        ((RectTransform)transform).anchoredPosition = new Vector2(
            (normalisedX - 0.5f) * HUDMarkerRect.rect.width,
            (normalisedY - 0.5f) * HUDMarkerRect.rect.height
        );
    }
}
