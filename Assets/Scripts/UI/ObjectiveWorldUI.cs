using UnityEngine.UI;
using UnityEngine;
using System;
using System.Collections;

public class ObjectiveWorldUI : MonoBehaviour
{
    [SerializeField] private Image progressBarImage;
    [SerializeField] private GameObject UI;
    [SerializeField, Range(0.5f, 10f)] private float fillSpeed = 1f;
    [SerializeField] private int objectiveIndex;

    private void Update()
    {
        UpdateProgressBar();
    }

    private void UpdateProgressBar()
    {
        //progressBarImage.fillAmount = GetComponent<ObjectiveInteract>().percentage;
        progressBarImage.fillAmount = Mathf.Lerp(progressBarImage.fillAmount, GetComponent<ObjectiveInteract>().percentage, Time.deltaTime * fillSpeed);

        if (progressBarImage.fillAmount >= 0.9)
        {
            //UI.SetActive(false);
            if(ObjectiveManager.Instance.CheckIndex(objectiveIndex)) ObjectiveManager.Instance.SetComplete(objectiveIndex);
            StartCoroutine(delayDisableUI());
        }
        else UI.SetActive(true);
    }
    
    private IEnumerator delayDisableUI()
    {
        yield return new WaitForSeconds(1f);
        UI.SetActive(false);
    }
}