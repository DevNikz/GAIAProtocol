using UnityEngine;
using UnityEngine.UI;

public class HUBLevelController : MonoBehaviour
{
    public void NextLevel()
    {
        HUBTransitioner.Instance.ToMechSelect();
        SoundManager.Instance.PlaySFX("Select1");
    }

    public void ToArmory()
    {
        HUBTransitioner.Instance.ToArmory();
        SoundManager.Instance.PlaySFX("Select3");
    }

    public void Return()
    {
        HUBTransitioner.Instance.Return();
        ReturnSFX();
    }

    public void ReturnSFX()
    {
        SoundManager.Instance.PlaySFX("Return");
    }

    public void CloseSFX_UI()
    {
        SettingsManager.Instance.CloseSFX();
    }

    public void Deploy()
    {
        HUBTransitioner.Instance.ToLevel();
        SoundManager.Instance.PlaySFX("Select1");
    }

    public void ToMechSelect()
    {
        HUBTransitioner.Instance.ToMechSelect();
        SoundManager.Instance.PlaySFX("Select3");
    }

    public void Select1_SFX()
    {
        SoundManager.Instance.PlaySFX("Select1");
    }

    public void Select3_SFX()
    {
        SoundManager.Instance.PlaySFX("Select3");
    }
}
