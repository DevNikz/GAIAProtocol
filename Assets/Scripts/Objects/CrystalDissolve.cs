using System.Collections;
using UnityEngine;

public class CrystalDissolveHDRP : MonoBehaviour
{
    public Renderer crystalRenderer;
    public ParticleSystem harvestParticles;
    public float dissolveDuration = 1.2f;
    public Color edgeColor = new Color(0.3f, 0.8f, 1f);
    public float maxEdgeIntensity = 5f;
    public AnimationCurve edgeIntensityCurve = AnimationCurve.EaseInOut(0, 0, 1, 0); // peaks mid-dissolve, fades at start/end

    private MaterialPropertyBlock mpb;
    private static readonly int DissolveID = Shader.PropertyToID("_DissolveAmount");
    private static readonly int EdgeColorID = Shader.PropertyToID("_EdgeColor");
    private static readonly int EdgeIntensityID = Shader.PropertyToID("_EdgeIntensity");

    void Awake()
    {
        mpb = new MaterialPropertyBlock();
    }

    public void Harvest()
    {
        StartCoroutine(DissolveRoutine());
    }

    IEnumerator DissolveRoutine()
    {
        if (harvestParticles)
            harvestParticles.Play();

        float t = 0f;
        while (t < dissolveDuration)
        {
            t += Time.deltaTime;
            float p = t / dissolveDuration;

            crystalRenderer.GetPropertyBlock(mpb);
            mpb.SetFloat(DissolveID, p);
            mpb.SetColor(EdgeColorID, edgeColor);
            mpb.SetFloat(EdgeIntensityID, edgeIntensityCurve.Evaluate(p) * maxEdgeIntensity);
            crystalRenderer.SetPropertyBlock(mpb);

            yield return null;
        }
        gameObject.SetActive(false);
    }
}
