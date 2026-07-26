using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalHarvestManager : MonoBehaviour
{
    [System.Serializable]
    public struct CrystalDissolveState
    {
        public Renderer renderer;
        public float timer;
        public float duration;
        public bool active;
    }

    public Transform crystalParent; // assign UnitWorldUI or wherever they live
    public float dissolveDuration = 1.2f;
    public Color edgeColor = new Color(0.3f, 0.8f, 1f);
    public float maxEdgeIntensity = 5f;
    public AnimationCurve edgeIntensityCurve = AnimationCurve.EaseInOut(0, 0.5f, 1, 0);
    public float staggerDelay = 0.08f;

    private static readonly int DissolveID = Shader.PropertyToID("_DissolveAmount");
    private static readonly int EdgeColorID = Shader.PropertyToID("_EdgeColor");
    private static readonly int EdgeIntensityID = Shader.PropertyToID("_EdgeIntensity");

    private List<Renderer> renderers = new List<Renderer>();
    private List<CrystalDissolveState> activeDissolves = new List<CrystalDissolveState>();
    private MaterialPropertyBlock mpb;

    public ParticleSystem harvestParticlePrefab;
    public int particlePoolSize = 6; // rarely need more than a few playing simultaneously

    private Queue<ParticleSystem> particlePool = new Queue<ParticleSystem>();

    void Awake()
    {
        mpb = new MaterialPropertyBlock();

        // Auto-collect all crystal renderers under the parent, no per-object script needed
        foreach (Transform child in crystalParent)
        {
            Renderer r = child.GetComponent<Renderer>();
            if (r != null)
                renderers.Add(r);
        }

        for (int i = 0; i < particlePoolSize; i++)
        {
            ParticleSystem ps = Instantiate(harvestParticlePrefab, transform);
            ps.gameObject.SetActive(false);
            particlePool.Enqueue(ps);
        }
    }

    private ParticleSystem GetPooledParticle()
    {
        if (particlePool.Count == 0)
            return null; // pool exhausted, skip particles this call
        ParticleSystem ps = particlePool.Dequeue();
        ps.gameObject.SetActive(true);
        return ps;
    }

    private void ReturnParticleWhenDone(ParticleSystem ps)
    {
        StartCoroutine(
            ReturnAfterDuration(ps, ps.main.duration + ps.main.startLifetime.constantMax)
        );
    }

    IEnumerator ReturnAfterDuration(ParticleSystem ps, float delay)
    {
        yield return new WaitForSeconds(delay);
        ps.gameObject.SetActive(false);
        particlePool.Enqueue(ps);
    }

    void Update()
    {
        for (int i = activeDissolves.Count - 1; i >= 0; i--)
        {
            var state = activeDissolves[i];
            state.timer += Time.deltaTime;
            float p = Mathf.Clamp01(state.timer / state.duration);

            state.renderer.GetPropertyBlock(mpb);
            mpb.SetFloat(DissolveID, p);
            mpb.SetColor(EdgeColorID, edgeColor);
            mpb.SetFloat(EdgeIntensityID, edgeIntensityCurve.Evaluate(p) * maxEdgeIntensity);
            state.renderer.SetPropertyBlock(mpb);

            if (p >= 1f)
            {
                state.renderer.gameObject.SetActive(false);
                activeDissolves.RemoveAt(i);
            }
            else
            {
                activeDissolves[i] = state;
            }
        }
    }

    public void HarvestAll()
    {
        for (int i = 0; i < renderers.Count; i++)
        {
            StartCoroutine(HarvestWithDelay(i, i * staggerDelay));
        }
    }

    IEnumerator HarvestWithDelay(int index, float delay)
    {
        yield return new WaitForSeconds(delay);
        HarvestCrystal(index);
    }

    // Call this from wherever your harvest action is triggered
    public void HarvestCrystal(int index)
    {
        if (index < 0 || index >= renderers.Count)
            return;

        activeDissolves.Add(
            new CrystalDissolveState
            {
                renderer = renderers[index],
                timer = 0f,
                duration = dissolveDuration,
                active = true,
            }
        );

        ParticleSystem ps = GetPooledParticle();
        if (ps != null)
        {
            ps.transform.position = renderers[index].bounds.center;
            ps.Play();
            ReturnParticleWhenDone(ps);
        }
    }

    // Or trigger by the specific renderer/gameobject if that's easier to reference from your turn logic
    public void HarvestCrystal(Renderer targetRenderer)
    {
        activeDissolves.Add(
            new CrystalDissolveState
            {
                renderer = targetRenderer,
                timer = 0f,
                duration = dissolveDuration,
                active = true,
            }
        );
    }
}
