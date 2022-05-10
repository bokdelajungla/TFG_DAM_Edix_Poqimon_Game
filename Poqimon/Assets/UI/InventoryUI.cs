using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    // Start is called before the first frame update
    public void HandleUpdate(Action onBack) {
        if (Input.GetKeyDown(KeyCode.X)) {
            onBack.Invoke();
        }
    }
}
