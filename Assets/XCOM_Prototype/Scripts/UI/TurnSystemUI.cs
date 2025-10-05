using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurnSystemUI : MonoBehaviour {

    private TextMeshProUGUI turnText;

    private void Awake() {
        turnText = transform.Find("TurnText").GetComponent<TextMeshProUGUI>();

        transform.Find("EndTurnBtn").GetComponent<Button>().onClick.AddListener(() => {
            TurnSystem.Instance.NextTurn();
        });
    }

    private void Start() {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;

        UpdateTurnText();
    }

    private void TurnSystem_OnTurnChanged(object sender, System.EventArgs e) {
        UpdateTurnText();
    }

    private void UpdateTurnText() {
        turnText.text = "TURN " + TurnSystem.Instance.GetTurnNumber();
    }

}