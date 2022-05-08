using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSelectionTracker : MonoBehaviour, ISelectHandler, IDeselectHandler {
    
    public bool IsSelected { get; private set; } = false;

    public void OnSelect(BaseEventData eventData)
    {
        IsSelected = true;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        IsSelected = false;
    }
}
