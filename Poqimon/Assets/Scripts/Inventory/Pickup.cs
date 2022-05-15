using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour, Interactable 
{

    [SerializeField] ItemBase item;
    [SerializeField] Dialog itemDialog;

    public bool used {get; set;} = false;
    public void Interact()
    {
        if (!used) {
        Debug.Log("Pickup is working");
        var player = FindObjectOfType<PlayerController>();
        player.GetComponent<Inventory>().addItem(item);
        used = true;

        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;

        StartCoroutine(DialogController.Instance.ShowDialog(itemDialog));
        }
        
    }
}
