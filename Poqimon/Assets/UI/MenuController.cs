using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject menu;
    List<Text> menuItems;
    int selectedItem = 0;

    public event Action<int> onMenuSelected;
    public event Action onBack;


    private void Awake() {
        menuItems = menu.GetComponentsInChildren<Text>().ToList();
    }
    public void openMenu() {
        menu.SetActive(true);
        updateItemSelection();
    }

    public void closeMenu() {
        menu.SetActive(false);
    }

    public void HandleUpdate() {

        int prevSelection = selectedItem;
        if (Input.GetKeyDown(KeyCode.DownArrow)) 
            ++selectedItem;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            --selectedItem;

        selectedItem = Mathf.Clamp(selectedItem, 0, menuItems.Count -1);

        if (prevSelection != selectedItem) {
            updateItemSelection();  
        }

        if (Input.GetKeyDown(KeyCode.Z)) {
            onMenuSelected.Invoke(selectedItem);
            closeMenu();
        } else if (Input.GetKeyDown(KeyCode.X)) {
            onBack.Invoke();
            closeMenu();
        }
    }

    public void updateItemSelection() {
        for (int i = 0; i < menuItems.Count; i++) {
            if (i == selectedItem)
                menuItems[i].color = Color.blue;
            else 
                menuItems[i].color = Color.black;
        }
    }

}
