using System.Collections.Generic;
using UnityEngine;

public class ToxicPuddle : MonoBehaviour
{
    [SerializeField] private int damagePerTurn = 10;
    [SerializeField] private int turnsRemaining = 3;
    [SerializeField] private GameObject vfxExpire; // optional pop effect

    private GridPosition gridPosition;
    //private int turnsRemaining;

    public void Initialize(GridPosition gridPosition, int turnsUntilExpiry, int damagePerTurn)
    {
        this.gridPosition = gridPosition;
        this.turnsRemaining = turnsUntilExpiry;
        this.damagePerTurn = damagePerTurn;

        // Register to turn system
        TurnSystem.Instance.OnTurnChanged += OnTurnChanged;
    }

    private void OnTurnChanged(object sender, System.EventArgs e)
    {
        // Only tick on the enemy turn (or every turn — your call)
        ApplyEffectsToUnitsOnTile();

        turnsRemaining--;
        if (turnsRemaining <= 0)
            Expire();
    }

    private void ApplyEffectsToUnitsOnTile()
    {
        if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(gridPosition)) return;

        Unit unit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        if (unit == null) return;
        if (unit.GetComponent<Unit>().HasCorruptionImmune()) return;

        // Damage
        if(unit.GetComponent<HealthSystem>().HasCorruptionResist()) unit.GetComponent<HealthSystem>().Damage(damagePerTurn / 2);
        else unit.GetComponent<HealthSystem>().Damage(damagePerTurn);

        // Status effect
        ToxicStatusEffect status = unit.GetComponent<ToxicStatusEffect>();
        if (status == null)
            status = unit.gameObject.AddComponent<ToxicStatusEffect>();

        status.Apply(turnsRemaining: 2, slowAmount: 0.5f);
    }

    private void Expire()
    {
        TurnSystem.Instance.OnTurnChanged -= OnTurnChanged;
        LevelGrid.Instance.UnregisterToxicPuddle(gridPosition); // see note below

        if (vfxExpire != null)
            Instantiate(vfxExpire, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    public GridPosition GetGridPosition() => gridPosition;
}