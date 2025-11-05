using UnityEngine.UI;
using UnityEngine;

public class ObjectiveWorldUI : MonoBehaviour
{
    [SerializeField] private Image progressBarImage;

    private void Update()
    {
        UpdateProgressBar();
    }

    private void UpdateProgressBar()
    {
        progressBarImage.fillAmount = GetComponent<ObjectiveInteract>().percentage;
    }
}