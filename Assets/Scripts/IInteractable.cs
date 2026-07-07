using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    void Interact(Action onInteractionComplete);
    void Interact(Action onInteractionComplete, float percentageAdd);
}
