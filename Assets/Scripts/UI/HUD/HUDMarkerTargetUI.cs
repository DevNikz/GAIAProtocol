using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;

public class HUDMarkerTargetUI : MonoBehaviour
{
    [SerializeField] private Image LinkedImage;
    
    private HUDMarkerInWorldTarget LinkedTarget;
    private bool isObjective;
    private RectTransform HUDMarkerRect;
    private int LinkedIndex; //For objective
    private Camera cam;

    private float CachedFOV_Horizontal = -1f;
    private float CachedFOV_Vertical = -1;
    private float CachedAspectRatio = -1f;

    public void Bind(HUDMarkerInWorldTarget _Target, Sprite _Image, bool _Objective, int _Index = 0)
    {
        LinkedTarget = _Target;
        LinkedImage.sprite = _Image;
        isObjective = _Objective;
        LinkedIndex = _Index;
    }

    void Start()
    {
        HUDMarkerRect = (RectTransform) transform.parent;
        cam = Camera.main;
        //cam = ObjectTransManager.Instance.GetMainCamera();

        if(!isObjective)
        {
            GetComponent<Image>().enabled = false;
            LinkedImage.enabled = false;
        }
    }

    void Update()
    {
        if(isObjective)
        {
            if(LinkedTarget == null || ObjectiveManager.Instance.GetComplete(LinkedIndex))
            {
                Destroy(gameObject);
                return;
            }
        }
        else
        {
            if (LinkedTarget == null)
            {
                Destroy(gameObject);
                return;
            }

            //For Extractions
            if(ObjectiveManager.Instance.CheckCompleteBool())
            {
                GetComponent<Image>().enabled = true;
                LinkedImage.enabled = true;
            }
        }

        var viewportPos = cam.WorldToViewportPoint(LinkedTarget.transform.position);
        
        if (viewportPos.x >= 0f && viewportPos.x <= 1 &&
            viewportPos.y >= 0f && viewportPos.y <= 1 &&
            viewportPos.z > 0)
        {
            OnScreenRepositionMarker();
        }

        else
        {
            OffScreenRepositionMarker();
        }
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
        Vector3 vecToTarget = (LinkedTarget.transform.position - Camera.main.transform.position).normalized;

        // refresh cached data if changed
        if (Camera.main.aspect != CachedAspectRatio || Camera.main.fieldOfView != CachedFOV_Vertical)
        {
            CachedFOV_Vertical = Camera.main.fieldOfView;
            CachedAspectRatio = Camera.main.aspect;

            CachedFOV_Horizontal = Camera.VerticalToHorizontalFieldOfView(CachedFOV_Vertical, CachedAspectRatio);
        }

        // calculate normalised angles to target
        float normalisedX = Mathf.Asin(Vector3.Dot(vecToTarget, Camera.main.transform.right)) * Mathf.Rad2Deg / (CachedFOV_Horizontal * 0.5f);
        float normalisedY = Mathf.Asin(Vector3.Dot(vecToTarget, Camera.main.transform.up)) * Mathf.Rad2Deg / (CachedFOV_Vertical * 0.5f);

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
        ((RectTransform)transform).anchoredPosition = new Vector2((normalisedX - 0.5f) * HUDMarkerRect.rect.width, 
                                                                  (normalisedY - 0.5f) * HUDMarkerRect.rect.height);
    }
}