using System.Collections.Generic;
using UnityEngine;

//Attach to Camera
public class ObjectTransManager : MonoBehaviour
{
    public static ObjectTransManager Instance;
    [SerializeReference] private Camera mainCam;
    [SerializeReference] private List<Transform> Units;
    [SerializeReference] private Vector3 offset;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private List<Transform> ObjectToHide = new List<Transform>();
    [SerializeField] private List<Transform> ObjectToShow = new List<Transform>();
    private Dictionary<Transform, Material> originalMaterials = new Dictionary<Transform, Material>();
    [SerializeField] private List<Material> transparentMaterial;
    [SerializeField] float obstructionFadingSpeed;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;

        mainCam = GetComponent<Camera>();
        Units.Clear();
    }

    private void LateUpdate()
    {
        if (Units.Count != 0) ManageBlockingView();
        else Debug.Log("Units not found");

        foreach (var obstruction in ObjectToHide)
        {
            HideObstruction(obstruction);
        }
        foreach (var obstruction in ObjectToShow)
        {
            ShowObstruction(obstruction);
        }
    }

    void ManageBlockingView()
    {
        for (int i = 0; i < Units.Count; i++)
        {
            Vector3 unitPos = Units[i].transform.position + offset;
            RaycastHit[] hits = Physics.RaycastAll(mainCam.transform.position, (Units[i].transform.position - mainCam.transform.position).normalized, Vector3.Distance(mainCam.transform.position, Units[i].transform.position), layerMask);
            if (hits.Length > 0)
            {
                Debug.Log("Obstructions Detected");
                // Repaint all the previous obstructions. Because some of the stuff might be not blocking anymore
                foreach (var obstruction in ObjectToHide)
                {
                    ObjectToShow.Add(obstruction);
                }

                ObjectToHide.Clear();

                // Hide the current obstructions
                foreach (var hit in hits)
                {
                    Transform obstruction = hit.transform;
                    ObjectToHide.Add(obstruction);
                    ObjectToShow.Remove(obstruction);
                    SetModeTransparent(obstruction);
                }
            }
            else
            {
                //Debug.DrawRay(mainCam.transform.position, (Units[i].transform.position - mainCam.transform.position).normalized * Vector3.Distance(mainCam.transform.position, Units[i].transform.position), Color.red);
                // Mean that no more stuff is blocking the view and sometimes all the stuff is not blocking as the same time
                foreach (var obstruction in ObjectToHide)
                {
                    ObjectToShow.Add(obstruction);
                }
                ObjectToHide.Clear();
            }
        }
    }

    void HideObstruction(Transform obj)
    {
        var color = obj.GetComponent<Renderer>().material.color;
        color.a = Mathf.Max(0, color.a - obstructionFadingSpeed * Time.deltaTime);
        obj.GetComponent<Renderer>().material.color = color;
    }

    void ShowObstruction(Transform obj)
    {
        var color = obj.GetComponent<Renderer>().material.color;
        color.a = Mathf.Min(1, color.a + obstructionFadingSpeed * Time.deltaTime);
        obj.GetComponent<Renderer>().material.color = color;
        if (Mathf.Approximately(color.a, 1f))
        {
            SetModeOpaque(obj);
        }
    }

    void SetModeTransparent(Transform tr)
    {
        MeshRenderer renderer = tr.GetComponent<MeshRenderer>();
        Material originalMat = renderer.material;

        // Material originalMat = renderer.sharedMaterial;
        if (!originalMaterials.ContainsKey(tr))
        {
            originalMaterials.Add(tr, originalMat);
        }
        else
        {
            return;
        }

        switch(renderer.material.name)
        {
            case "colormap":
            case "colormap (Instance)":
                Material materialTrans = new Material(transparentMaterial[0]);
                renderer.material = materialTrans;
                renderer.material.mainTexture = originalMat.mainTexture;
                break;
        }
        //materialTrans.CopyPropertiesFromMaterial(originalMat);
    }

    void SetModeOpaque(Transform tr)
    {
        //MeshRenderer renderer = tr.GetComponent<MeshRenderer>();
        //renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;

        if (originalMaterials.ContainsKey(tr))
        {
            tr.GetComponent<MeshRenderer>().material = originalMaterials[tr];
            originalMaterials.Remove(tr);
        }
    }

    public void AddUnit(Transform unit)
    {
        Units.Add(unit);
    }
}
