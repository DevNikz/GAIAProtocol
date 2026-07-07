using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public enum TierTypeUpgrade
{
    Tier1_W,
    Tier2_W,
    Tier3_W,
    Tier1_R,
    Tier2_R,
    Tier3_R,
}

public class MechSelecter : MonoBehaviour
{
    [SerializeField]
    FriendlyUnitType currentUnit;

    [SerializeField]
    Transform target;

    [SerializeField]
    GameObject upgradesParentWorker;

    [SerializeField]
    GameObject upgradesParentRanger;

    [SerializeField]
    GameObject workerModel;

    [SerializeField]
    GameObject rangerModel;

    [SerializeField]
    ParticleSystem levelUpParticle;
    Transform cam;

    [SerializeField]
    float speed;

    public FriendlyUnitType GetCurrentUnit()
    {
        return currentUnit;
    }

    public void SetSelectedUnit(FriendlyUnitType type)
    {
        currentUnit = type;
    }

    void Awake()
    {
        cam = Camera.main.transform;
        target = transform.Find("Content/Garage/Mech");
        levelUpParticle = transform.Find("Content/Garage/LevelUp").GetComponent<ParticleSystem>();
        currentUnit = FriendlyUnitType.WORKER;
    }

    public void ToUpgrades()
    {
        //true = worker
        //false = ranger

        if (CheckSelectedUnit())
        {
            upgradesParentWorker.SetActive(true);
            upgradesParentRanger.SetActive(false);
        }
        else
        {
            upgradesParentWorker.SetActive(false);
            upgradesParentRanger.SetActive(true);
        }
    }

    bool CheckSelectedUnit()
    {
        if (GetCurrentUnit() == FriendlyUnitType.WORKER)
            return true;
        else
            return false;
    }

    public void SwitchCurrentUnit()
    {
        if (CheckSelectedUnit())
        {
            SetSelectedUnit(FriendlyUnitType.RANGER);
        }
        else
            SetSelectedUnit(FriendlyUnitType.WORKER);

        SetCurrentModel();
    }

    void SetCurrentModel()
    {
        if (GetCurrentUnit() == FriendlyUnitType.WORKER)
        {
            workerModel.SetActive(true);
            rangerModel.SetActive(false);
        }
        else
        {
            workerModel.SetActive(false);
            rangerModel.SetActive(true);
        }
    }

    void Update()
    {
        RotateMech();
    }

    void RotateMech()
    {
        transform.Rotate(Vector3.up, InputManager.Instance.GetRotateY() * speed);
    }

    public void BuyUpgradeWorkerT2(int cost)
    {
        if (CurrencyManager.Instance.GetResearchPoints() >= cost)
        {
            //buy
            levelUpParticle.Play();
            SoundManager.Instance.PlaySFX("Select3");
            CurrencyManager.Instance.SetResearchPoints(
                CurrencyManager.Instance.GetResearchPoints() - cost
            );
            MechManager.Instance.UpgradeWorker(WorkerTier.TIER2);
        }
        else
        {
            SoundManager.Instance.PlaySFX("Delete");
        }
    }

    public void BuyUpgradeWorkerT3(int cost)
    {
        if (CurrencyManager.Instance.GetResearchPoints() >= cost)
        {
            //buy
            levelUpParticle.Play();
            SoundManager.Instance.PlaySFX("Select3");
            CurrencyManager.Instance.SetResearchPoints(
                CurrencyManager.Instance.GetResearchPoints() - cost
            );
            MechManager.Instance.UpgradeWorker(WorkerTier.TIER3);
        }
        else
        {
            SoundManager.Instance.PlaySFX("Delete");
        }
    }

    public void BuyUpgradeRangerT2(int cost)
    {
        if (CurrencyManager.Instance.GetResearchPoints() >= cost)
        {
            //buy
            levelUpParticle.Play();
            SoundManager.Instance.PlaySFX("Select3");
            CurrencyManager.Instance.SetResearchPoints(
                CurrencyManager.Instance.GetResearchPoints() - cost
            );
            MechManager.Instance.UpgradeRanger(RangerTier.TIER2);
        }
        else
        {
            SoundManager.Instance.PlaySFX("Delete");
        }
    }

    public void BuyUpgradeRangerT3(int cost)
    {
        if (CurrencyManager.Instance.GetResearchPoints() >= cost)
        {
            //buy
            levelUpParticle.Play();
            SoundManager.Instance.PlaySFX("Select3");
            CurrencyManager.Instance.SetResearchPoints(
                CurrencyManager.Instance.GetResearchPoints() - cost
            );
            MechManager.Instance.UpgradeRanger(RangerTier.TIER3);
        }
        else
        {
            SoundManager.Instance.PlaySFX("Delete");
        }
    }
}
