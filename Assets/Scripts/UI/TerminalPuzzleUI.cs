using System;
using System.Collections.Generic;
using UnityEngine;

public class TerminalPuzzleUI : MonoBehaviour
{
    public static TerminalPuzzleUI Instance { get; private set; }

    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject mainUI;
    [SerializeField] private GameObject UIBlur;

    public bool isPuzzle;

    public event EventHandler OnPuzzleComplete;

    [SerializeReference] private string text;
    [SerializeReference] private List<string> arrowComboList;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        HidePuzzleUI();
    }

    public void ShowPuzzleUI()
    {
        isPuzzle = true;
        panel.SetActive(true);
        UIBlur.SetActive(true);
        mainUI.SetActive(false);
    }

    public void HidePuzzleUI()
    {
        isPuzzle = false;
        panel.SetActive(false);
        UIBlur.SetActive(false);
        mainUI.SetActive(true);
    }


    //Terminal Puzzle thingy

    void Update()
    {
        if(isPuzzle) DebugPrintArrowKeys();
    }

    void DebugPrintArrowKeys()
    {
        if (arrowComboList.Count == 4)
        {
            Debug.Log("Puzzle Complete!");
            OnPuzzleComplete?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.RightArrow)) AddArrowKey("Right");
            else if (Input.GetKeyDown(KeyCode.LeftArrow)) AddArrowKey("Left");
            else if (Input.GetKeyDown(KeyCode.UpArrow)) AddArrowKey("Up");
            else if (Input.GetKeyDown(KeyCode.DownArrow)) AddArrowKey("Down");
        }
    }

    void AddArrowKey(string value)
    {
        arrowComboList.Add(value);
        Debug.Log($"Current Arrow Key Combo: {arrowComboList[arrowComboList.Count - 1]}");
    }

}