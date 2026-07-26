using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class RadarScanEffect : MonoBehaviour
{
    public CustomPassVolume passVolume; // drag your Custom Pass Volume
    public float maxRadius = 150f;
    public float duration = 2.5f;
    public AnimationCurve radiusCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private static readonly int OriginID = Shader.PropertyToID("_Origin");
    private static readonly int RadiusID = Shader.PropertyToID("_Radius");
    private static readonly int MaxRadiusID = Shader.PropertyToID("_MaxRadius");
    private static readonly int FadeID = Shader.PropertyToID("_Fade");

    private FullScreenCustomPass _pass;
    private Material _mat;

    void Awake()
    {
        var pass = passVolume.customPasses[0] as FullScreenCustomPass;
        _mat = pass.fullscreenPassMaterial;
    }

    public void TriggerScan(Vector3 worldOrigin)
    {
        StopAllCoroutines();
        StartCoroutine(ScanRoutine(worldOrigin));
    }

    public void ResetMaterial()
    {
        StopAllCoroutines();
        _mat.SetFloat(RadiusID, 0f);
        _mat.SetFloat(FadeID, 0f);
    }

    private System.Collections.IEnumerator ScanRoutine(Vector3 origin)
    {
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            float p = t / duration;
            float radius = radiusCurve.Evaluate(p) * maxRadius;

            _mat.SetVector(OriginID, origin);
            _mat.SetFloat(RadiusID, radius);
            _mat.SetFloat(MaxRadiusID, maxRadius);
            _mat.SetFloat(FadeID, 1 - p);

            yield return null;
        }

        ResetMaterial();
    }
}
