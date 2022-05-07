using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] Dialog npcDialog;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Interact()
    {
        StartCoroutine(DialogController.Instance.ShowDialog(npcDialog));
    }
}
