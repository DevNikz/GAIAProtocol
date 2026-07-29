using System.Collections.Generic;
using UnityEngine;

namespace ForestBiome
{
    /// <summary>
    /// Config + runtime state for one pipe, referenced from the OilFlowManager
    /// list below. This replaces the per-pipe OilFlowController component —
    /// remove that component from your pipes and fill in one entry per pipe
    /// here instead.
    /// </summary>
    [System.Serializable]
    public class OilPipeEntry
    {
        [Tooltip("Used to look up a pipe from code, e.g. manager.BeginOverflow(\"PipeA\").")]
        public string id = "Pipe";

        [Tooltip(
            "Used for distance-based drip culling. Usually the pipe's own transform or the trough lip."
        )]
        public Transform pipeTransform;

        [Tooltip("Renderers using the M_OilFlow material (pool + trough surfaces).")]
        public Renderer[] oilSurfaceRenderers;

        public ParticleSystem dripParticles;

        [Header("Flow settings")]
        [Range(0f, 1f)]
        public float targetFillAmount = 1f;
        public float flowSpeed = 0.6f;
        public float fillRampDuration = 2f;
        public float dripCullDistance = 30f;

        // Runtime state — not authored in the inspector, reset by the manager.
        [System.NonSerialized]
        public float rampTimer;

        [System.NonSerialized]
        public bool dripsEnabled = true;

        [System.NonSerialized]
        public bool isOverflowing;
    }

    /// <summary>
    /// Drives every oil pipe in the level from a single Update loop. Handles
    /// the fill ramp animation and drip-particle LOD culling for each pipe
    /// entry, sharing one MaterialPropertyBlock instance across all of them
    /// (it's reused/overwritten per-pipe, not reallocated).
    /// </summary>
    public class OilFlowManager : MonoBehaviour
    {
        [SerializeField]
        private List<OilPipeEntry> pipes = new List<OilPipeEntry>();

        private static readonly int FillAmountId = Shader.PropertyToID("_FillAmount");
        private static readonly int FlowSpeedId = Shader.PropertyToID("_FlowSpeed");

        private MaterialPropertyBlock _propertyBlock;
        private Transform _cameraTransform;

        private void Awake()
        {
            _propertyBlock = new MaterialPropertyBlock();
            if (Camera.main != null)
                _cameraTransform = Camera.main.transform;
        }

        private void Update()
        {
            // Camera.main can be null for a frame or two on scene load; keep
            // retrying cheaply rather than caching a null reference forever.
            if (_cameraTransform == null && Camera.main != null)
            {
                _cameraTransform = Camera.main.transform;
            }

            for (int i = 0; i < pipes.Count; i++)
            {
                UpdatePipe(pipes[i]);
            }
        }

        private void UpdatePipe(OilPipeEntry pipe)
        {
            if (pipe.isOverflowing && pipe.rampTimer < pipe.fillRampDuration)
            {
                pipe.rampTimer += Time.deltaTime;
                float t = Mathf.Clamp01(pipe.rampTimer / pipe.fillRampDuration);
                ApplyFill(pipe, t * pipe.targetFillAmount);
            }

            UpdateDripLOD(pipe);
        }

        private void ApplyFill(OilPipeEntry pipe, float value)
        {
            if (pipe.oilSurfaceRenderers == null)
                return;

            _propertyBlock.SetFloat(FillAmountId, value);
            _propertyBlock.SetFloat(FlowSpeedId, pipe.flowSpeed);

            for (int i = 0; i < pipe.oilSurfaceRenderers.Length; i++)
            {
                var renderer = pipe.oilSurfaceRenderers[i];
                if (renderer == null)
                    continue;
                renderer.SetPropertyBlock(_propertyBlock);
            }
        }

        private void UpdateDripLOD(OilPipeEntry pipe)
        {
            if (
                pipe.dripParticles == null
                || _cameraTransform == null
                || pipe.pipeTransform == null
            )
                return;

            float sqrDist = (pipe.pipeTransform.position - _cameraTransform.position).sqrMagnitude;
            bool shouldEmit = sqrDist <= pipe.dripCullDistance * pipe.dripCullDistance;

            if (shouldEmit == pipe.dripsEnabled)
                return; // no state change, skip the call

            pipe.dripsEnabled = shouldEmit;
            var emission = pipe.dripParticles.emission;
            emission.enabled = shouldEmit;

            if (!shouldEmit)
                pipe.dripParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            else if (!pipe.dripParticles.isPlaying)
                pipe.dripParticles.Play();
        }

        /// <summary>Start (or restart) an overflow on a pipe found by its id.</summary>
        public void BeginOverflow(string id, float targetFill = 1f, float rampDuration = 2f)
        {
            var pipe = pipes.Find(p => p.id == id);
            if (pipe == null)
            {
                Debug.LogWarning($"OilFlowManager: no pipe entry with id '{id}'.");
                return;
            }
            BeginOverflow(pipe, targetFill, rampDuration);
        }

        /// <summary>Start (or restart) an overflow on a pipe found by list index.</summary>
        public void BeginOverflow(int index, float targetFill = 1f, float rampDuration = 2f)
        {
            if (index < 0 || index >= pipes.Count)
                return;
            BeginOverflow(pipes[index], targetFill, rampDuration);
        }

        private void BeginOverflow(OilPipeEntry pipe, float targetFill, float rampDuration)
        {
            pipe.targetFillAmount = targetFill;
            pipe.fillRampDuration = rampDuration;
            pipe.rampTimer = 0f;
            pipe.isOverflowing = true;
        }

        /// <summary>Immediately empty a pipe and stop its overflow state.</summary>
        public void StopOverflow(string id)
        {
            var pipe = pipes.Find(p => p.id == id);
            if (pipe == null)
                return;

            pipe.isOverflowing = false;
            pipe.rampTimer = 0f;
            ApplyFill(pipe, 0f);
        }
    }
}
