using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Attach to Camera
public class ObjectTransManager : MonoBehaviour
{
    public static ObjectTransManager Instance;
    [SerializeField] private Camera mainCam;
    [SerializeField] private List<Transform> Units;
    [SerializeField] private Vector3 offset;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private List<Transform> ObjectToHide = new List<Transform>();
    [SerializeField] private List<Transform> ObjectToShow = new List<Transform>();
    [SerializeField] private Dictionary<Transform, Material> originalMaterials = new Dictionary<Transform, Material>();
    [SerializeField] private List<Material> transparentMaterial;
    [SerializeField] float obstructionFadingSpeed;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
        }
        else Destroy(gameObject);

        Units.Clear();
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch(scene.buildIndex)
        {
            case 0:
                Units.Clear();
                break;
            case 1:
            case 2:
            case 3:
                mainCam = Camera.main;
                Units.Clear();
                break;
        }
    }

    private void LateUpdate()
    {
        if(SceneManager.GetActiveScene().buildIndex != 0)
        {
            DoUpdate();
        }
    }

    void DoUpdate()
    {
        if (Units.Count != 0) ManageBlockingView();
        //else Debug.Log("Units not found");

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
            RaycastHit[] hits = Physics.RaycastAll(mainCam.transform.position, (unitPos - mainCam.transform.position).normalized, Vector3.Distance(mainCam.transform.position, unitPos), layerMask);
            //RaycastHit[] hits = Physics.RaycastAll(mainCam.transform.position, (Units[i].transform.position - mainCam.transform.position).normalized, Vector3.Distance(mainCam.transform.position, Units[i].transform.position), layerMask);
            if (hits.Length > 0)
            {
                //Debug.Log("Obstructions Detected");
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
        var color = obj.GetComponent<Renderer>().material.GetColor("_BaseColor");
        color.a = Mathf.Max(0, color.a - obstructionFadingSpeed * Time.deltaTime);
        obj.GetComponent<Renderer>().material.SetColor("_BaseColor", color);
        //var color = obj.GetComponent<Renderer>().material.GetColor("Albedo");
        //var color = obj.GetComponent<Renderer>().material.color;
        //var color = obj.GetComponent<Renderer>().material.GetColor("BaseColor");
        //color.a = Mathf.Max(0, color.a - obstructionFadingSpeed * Time.deltaTime);
        //obj.GetComponent<Renderer>().material.color = color;
        //obj.GetComponent<Renderer>().material.SetColor("Albedo", color);
        //obj.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.black);
    }

    void ShowObstruction(Transform obj)
    {
        Color color = Color.white;
        if(obj.GetComponent<Renderer>().material.name == transparentMaterial[0].name + " (Instance)" ||
        obj.GetComponent<Renderer>().material.name == transparentMaterial[1].name + " (Instance)")
        {
            color = obj.GetComponent<Renderer>().material.GetColor("_BaseColor");
            color.a = Mathf.Min(1, color.a + obstructionFadingSpeed * Time.deltaTime);
            obj.GetComponent<Renderer>().material.SetColor("_BaseColor", color);
            if (Mathf.Approximately(color.a, 1f))
            {
                SetModeOpaque(obj);
            }
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
            case "foliage":
            case "foliage (Instance)":
                Material foliageTrans = new Material(transparentMaterial[0]);
                renderer.material = foliageTrans;
                break;
            case "bark":
            case "bark (Instance)":
                // Material barkTrans = new Material(transparentMaterial[1]);
                // renderer.material = barkTrans;
                break;
                //renderer.material.mainTexture = originalMat.mainTexture;
            // case "colormap":
            // case "colormap (Instance)":
            //     Material materialTrans = new Material(transparentMaterial[0]);
            //     renderer.material = materialTrans;
            //     renderer.material.mainTexture = originalMat.mainTexture;
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

    public void RemoveUnit(Transform unit)
    {
        Units.Remove(unit);
    }

    public Camera GetMainCamera()
    {
        return mainCam;
    }
}
