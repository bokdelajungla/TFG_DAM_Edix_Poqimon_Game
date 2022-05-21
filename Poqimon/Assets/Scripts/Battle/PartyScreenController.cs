using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreenController : MonoBehaviour
{
    [SerializeField] Text messageTxt;

    PartyMemberController[] memberSlots;
    List<Poqimon> partyPoqimons;

    private int currentPoqimon = 0;
    private int selection = 0;

    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberController>(true);
    }

    public void SetPartyData(List<Poqimon> partyPoqimons)
    {
        this.partyPoqimons = partyPoqimons;

        for (int i = 0; i<memberSlots.Length; i++)
        {
            if (i < partyPoqimons.Count)
            {
                memberSlots[i].gameObject.SetActive(true);
                memberSlots[i].SetData(partyPoqimons[i]);
            }
            else
            {
                memberSlots[i].gameObject.SetActive(false);
            }
        }

        messageTxt.text = "Choose a Poqimon";
    }

    public void HandleUpdate(Action onSelected, Action onBack)
    {
        var prevSelection = selection;
        
        if (Input.GetKeyDown(KeyCode.RightArrow))
            currentPoqimon++;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            currentPoqimon--;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentPoqimon++;
        else if(Input.GetKeyDown(KeyCode.UpArrow))
            currentPoqimon--;
        currentPoqimon = Mathf.Clamp(currentPoqimon, 0, partyPoqimons.Count - 1);
        
        UpdatePartySelection(currentPoqimon);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            onSelected?.Invoke();
            /*
            var selectedMember = playerParty.Party[currentMember];
            if (selectedMember.CurrentHp <= 0){
                partyScreenController.SetMessageText("Poqimon is fainted and cannot fight!");
                return;
            }
            if (selectedMember == playerUnit.Poqimon)
            {
                partyScreenController.SetMessageText(selectedMember.PoqimonBase.PoqimonName + " is already fighting!");
                return;
            }
            partyScreenController.gameObject.SetActive(false);
            state = BattleState.Busy;
            StartCoroutine(SwitchPoqimon(selectedMember));
            */
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            onBack?.Invoke();
            /*
            partyScreenController.gameObject.SetActive(false);
            dialog.EnableDialogTxt(true);
            ActionSelection();
            */
        }
    }
    
    public void UpdatePartySelection(int selectedMember)
    {
        for (int i = 0; i < partyPoqimons.Count; i++)
        {
            if (i == selectedMember)
                memberSlots[i].SetSelected(true);
            else
                memberSlots[i].SetSelected(false);
        }
    }

    public void SetMessageText(string message)
    {
        messageTxt.text = message;
    }
}
