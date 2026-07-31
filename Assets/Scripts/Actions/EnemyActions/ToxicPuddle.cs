using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToxicPuddle : MonoBehaviour
{
    [SerializeField]
    private int damagePerTurn = 10;

    [SerializeField]
    private int turnsRemaining = 3;

    [SerializeField]
    private GameObject vfxExpire; // optional pop effect

    [SerializeField]
    private float fadeDuration = 1f;

    [SerializeField]
    bool hasExpiry = true;

    private GridPosition gridPosition;
    private Renderer puddleRenderer;
    private Material puddleMaterialInstance;
    private MaterialPropertyBlock propBlock;
    private readonly HashSet<Unit> unitsInPuddle = new HashSet<Unit>();
    private static readonly int customFadeId = Shader.PropertyToID("_Edge_Fade_Alpha");
    private static readonly int customColorId2 = Shader.PropertyToID("_Color2");
    private System.Action<ToxicPuddle> releaseToPool;

    private void Awake()
    {
        puddleRenderer = GetComponent<Renderer>();
        if (puddleRenderer != null)
        {
            puddleMaterialInstance = puddleRenderer.material;
        }
        propBlock = new MaterialPropertyBlock();
    }

    public void SetPoolReleaseCallback(System.Action<ToxicPuddle> releaseToPool)
    {
        this.releaseToPool = releaseToPool;
    }

    public void Initialize(GridPosition gridPosition, int turnsUntilExpiry, int damagePerTurn)
    {
        this.gridPosition = gridPosition;
        this.turnsRemaining = turnsUntilExpiry;
        this.damagePerTurn = damagePerTurn;

        ResetVisuals();

        // Register to turn system
        TurnSystem.Instance.OnTurnChanged -= OnTurnChanged;
        TurnSystem.Instance.OnTurnChanged += OnTurnChanged;
    }

    private void ResetVisuals()
    {
        if (puddleMaterialInstance == null)
            return;

        int fadeId = puddleMaterialInstance.HasProperty(customFadeId) ? customFadeId : customFadeId;
        if (!puddleMaterialInstance.HasProperty(fadeId))
            return;

        float f = puddleMaterialInstance.GetFloat(fadeId);
        f = 4.5f;
        puddleMaterialInstance.SetFloat(fadeId, f);
        // Color c = puddleMaterialInstance.GetColor(colorId);
        // c.a = 1f;
        // puddleMaterialInstance.SetColor(colorId, c);
    }

    private void OnTurnChanged(object sender, System.EventArgs e)
    {
        // Only tick on the enemy turn (or every turn — your call)
        ApplyEffectsToUnitsOnTile();

        turnsRemaining--;
        if (turnsRemaining <= 0 && hasExpiry == true)
            Expire();
    }

    private void ApplyEffectsToUnitsOnTile()
    {
        if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(gridPosition))
            return;

        Unit unit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        if (unit == null)
            return;
        if (unit.GetComponent<Unit>().HasCorruptionImmune())
            return;

        // Damage
        if (unit.GetComponent<HealthSystem>().HasCorruptionResist())
            unit.GetComponent<HealthSystem>().Damage(damagePerTurn / 2);
        else
            unit.GetComponent<HealthSystem>().Damage(damagePerTurn);

        // Status effect
        ToxicStatusEffect status = unit.GetComponent<ToxicStatusEffect>();
        if (status == null)
            status = unit.gameObject.AddComponent<ToxicStatusEffect>();

        status.Apply(turnsRemaining: 2, slowAmount: 0.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        Unit unit = other.GetComponent<Unit>();
        if (unit == null)
            return;

        if (unitsInPuddle.Contains(unit))
            return; // already applied for this stay in the puddle

        unitsInPuddle.Add(unit);
        ApplyEffectsToUnit(unit);
    }

    private void OnTriggerExit(Collider other)
    {
        Unit unit = other.GetComponentInParent<Unit>();
        if (unit == null)
            return;

        unitsInPuddle.Remove(unit);
    }

    private void ApplyEffectsToUnit(Unit unit)
    {
        if (unit.HasCorruptionImmune())
            return;

        // Damage
        HealthSystem healthSystem = unit.GetComponent<HealthSystem>();
        if (healthSystem.HasCorruptionResist())
            healthSystem.Damage(damagePerTurn / 2);
        else
            healthSystem.Damage(damagePerTurn);

        // Status effect
        ToxicStatusEffect status = unit.GetComponent<ToxicStatusEffect>();
        if (status == null)
            status = unit.gameObject.AddComponent<ToxicStatusEffect>();

        status.Apply(turnsRemaining: 2, slowAmount: 0.5f);
    }

    public void Expire()
    {
        TurnSystem.Instance.OnTurnChanged -= OnTurnChanged;
        LevelGrid.Instance.UnregisterToxicPuddle(gridPosition); // see note below

        if (vfxExpire != null)
            Instantiate(vfxExpire, transform.position, Quaternion.identity);

        if (puddleRenderer != null && fadeDuration > 0f)
            StartCoroutine(FadeOutAndDestroy());
        else
            ReleaseOrDestroy();
    }

    private IEnumerator FadeOutAndDestroy()
    {
        int fadeId = puddleMaterialInstance.HasProperty(customFadeId) ? customFadeId : customFadeId;

        float edgeFade = puddleMaterialInstance.HasProperty(fadeId)
            ? puddleMaterialInstance.GetFloat(fadeId)
            : 1f;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            float alpha = Mathf.Lerp(edgeFade, 0f, t);

            float faded = edgeFade;
            faded = alpha;
            puddleMaterialInstance.SetFloat(fadeId, faded);

            // Color faded = startColor;
            // faded.a = alpha;
            // mat.SetColor(colorId, faded);

            yield return null;
        }

        ReleaseOrDestroy();
    }

    private void ReleaseOrDestroy()
    {
        if (releaseToPool != null)
            releaseToPool(this);
        else
            Destroy(gameObject);
    }

    public GridPosition GetGridPosition() => gridPosition;
}
