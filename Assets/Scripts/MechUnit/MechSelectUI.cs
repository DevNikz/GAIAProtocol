using UnityEngine;

public class MechSelectUI : MonoBehaviour
{

    public FriendlyUnitType type;
    public void RemoveWorkerUnit()
    {
        SoundManager.Instance.PlaySFX("Delete");
        transform.root.GetComponent<MechDeploymentUI>().RemoveWorkerUnit();
        transform.root.GetComponent<MechDeploymentUI>().RemoveAtIndex(gameObject);
        Destroy(gameObject);
    }

    public void RemoveRangerUnit()
    {
        SoundManager.Instance.PlaySFX("Delete");
        transform.root.GetComponent<MechDeploymentUI>().RemoveRangerUnit();
        transform.root.GetComponent<MechDeploymentUI>().RemoveAtIndex(gameObject);
        Destroy(gameObject);
    }

    public void EnableUnitSelection()
    {
        SoundManager.Instance.PlaySFX("Select3");
        transform.root.transform.Find("MainUI/MechDeployment/Content").gameObject.SetActive(false);
        transform.root.transform.Find("MainUI/MechSelect/Content").gameObject.SetActive(true);
    }

    public void DisableUnitSelection()
    {
        transform.root.transform.Find("MainUI/MechDeployment/Content").gameObject.SetActive(true);
        transform.root.transform.Find("MainUI/MechSelect/Content").gameObject.SetActive(false);
    }

    public void ClearAll()
    {
        MechManager.Instance.ClearAll();
    }

    public void AddWorkerUnit()
    {
        transform.root.GetComponent<MechDeploymentUI>().AddWorkerUnit();
        DisableUnitSelection();
    }

    public void AddRangerUnit()
    {
        transform.root.GetComponent<MechDeploymentUI>().AddRangerUnit();
        DisableUnitSelection();
    }
}
