using UnityEngine;

namespace ForestBiome
{
    /// <summary>
    /// Drives one overflowing pipe: animates the oil shader's fill/flow via a
    /// MaterialPropertyBlock (no per-pipe material instances, stays SRP Batcher
    /// friendly), and LODs out the drip particle system when the pipe is far
    /// from the camera. The shader itself keeps flowing at effectively zero
    /// cost when far away — only the particle sim is worth culling.
    ///
    /// For many pipes at once, don't attach one of these per pipe; instead
    /// pull the per-instance logic into a manager that iterates a list and
    /// calls the static ApplyFill/ApplyLOD helpers directly, so you get one
    /// Update() instead of N.
    /// </summary>
    [DisallowMultipleComponent]
    public class OilFlowController : MonoBehaviour
    {
        [Header("Oil surfaces (pool + trough share the same M_OilFlow material)")]
        [SerializeField] private Renderer[] oilSurfaceRenderers;

        [Header("Drip particles at the trough lip")]
        [SerializeField] private ParticleSystem dripParticles;
        [SerializeField] private float dripCullDistance = 30f;

        [Header("Flow state")]
        [Range(0f, 1f)] [SerializeField] private float fillAmount = 1f;
        [SerializeField] private float flowSpeed = 0.6f;
        [SerializeField] private float fillRampDuration = 2f; // seconds to animate 0 -> 1 on start

        private static readonly int FillAmountId = Shader.PropertyToID("_FillAmount");
        private static readonly int FlowSpeedId = Shader.PropertyToID("_FlowSpeed");

        private MaterialPropertyBlock _propertyBlock;
        private Transform _cameraTransform;
        private float _rampTimer;
        private bool _dripsEnabled = true;

        private void Awake()
        {
            _propertyBlock = new MaterialPropertyBlock();
            if (Camera.main != null) _cameraTransform = Camera.main.transform;
        }

        private void OnEnable()
        {
            _rampTimer = 0f;
            ApplyFill(0f);
        }

        private void Update()
        {
            // Animate the fill ramp once, then stop touching the property
            // block entirely (avoids per-frame SetPropertyBlock calls once
            // the pipe has settled into steady overflow).
            if (_rampTimer < fillRampDuration)
            {
                _rampTimer += Time.deltaTime;
                float t = Mathf.Clamp01(_rampTimer / fillRampDuration);
                ApplyFill(t * fillAmount);
            }

            UpdateDripLOD();
        }

        private void ApplyFill(float value)
        {
            if (oilSurfaceRenderers == null) return;

            _propertyBlock.SetFloat(FillAmountId, value);
            _propertyBlock.SetFloat(FlowSpeedId, flowSpeed);

            for (int i = 0; i < oilSurfaceRenderers.Length; i++)
            {
                if (oilSurfaceRenderers[i] == null) continue;
                oilSurfaceRenderers[i].SetPropertyBlock(_propertyBlock);
            }
        }

        private void UpdateDripLOD()
        {
            if (dripParticles == null || _cameraTransform == null) return;

            float sqrDist = (transform.position - _cameraTransform.position).sqrMagnitude;
            bool shouldEmit = sqrDist <= dripCullDistance * dripCullDistance;

            if (shouldEmit == _dripsEnabled) return; // no state change, skip the call

            _dripsEnabled = shouldEmit;
            var emission = dripParticles.emission;
            emission.enabled = shouldEmit;

            if (!shouldEmit) dripParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            else if (!dripParticles.isPlaying) dripParticles.Play();
        }

        /// <summary>Call to (re)start an overflow, e.g. when triggered by gameplay.</summary>
        public void BeginOverflow(float targetFill = 1f, float rampDuration = 2f)
        {
            fillAmount = targetFill;
            fillRampDuration = rampDuration;
            _rampTimer = 0f;
        }
    }
}
