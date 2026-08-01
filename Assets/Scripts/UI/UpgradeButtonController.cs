using UnityEngine;
using UnityEngine.UI;

public class UpgradeButtonController : MonoBehaviour
{
    [SerializeField]
    int tier;

    [SerializeField]
    MechType type;

    [SerializeField]
    GameObject Cost,
        Unlocked;

    [SerializeField]
    Button button;

    [SerializeField]
    Button button2;

    public void LateUpdate()
    {
        //Worker
        if (type == MechType.WORKER && MechManager.Instance.GetTierWorkerUnlockState(tier))
        {
            Cost.SetActive(false);
            Unlocked.SetActive(true);
            button.interactable = false;
            // if (button2 != null)
            //     button2.interactable = true;
        }
        else if (type == MechType.WORKER && !MechManager.Instance.GetTierWorkerUnlockState(tier))
        {
            Cost.SetActive(true);
            Unlocked.SetActive(false);
            button.interactable = true;
            // if (button2 != null)
            //     button2.interactable = false;
        }

        //Ranger
        if (type == MechType.RANGER && MechManager.Instance.GetTierRangerUnlockState(tier))
        {
            Cost.SetActive(false);
            Unlocked.SetActive(true);
            button.interactable = false;
            // if (button2 != null)
            //     button2.interactable = true;
        }
        else if (type == MechType.RANGER && !MechManager.Instance.GetTierRangerUnlockState(tier))
        {
            Cost.SetActive(true);
            Unlocked.SetActive(false);
            button.interactable = true;
            // if (button2 != null)
            //     button2.interactable = false;
        }
    }
}
