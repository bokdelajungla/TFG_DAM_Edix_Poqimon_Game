using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreenController : MonoBehaviour
{
    [SerializeField] Text messageTxt;

    PartyMemberController[] memberSlots;
    List<Poqimon> partyPoqimons;
    PoqimonParty poqimonParty;

    int selection = 0;
    public Poqimon SelectedMember => partyPoqimons[selection];

    /// Party screen can be called from different states like ActionSelection, RunningTurn, AboutToUse
    public BattleState? CalledFrom { get; set; }

    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberController>(true);
        poqimonParty = PoqimonParty.GetPlayerParty();
        SetPartyData();

        poqimonParty.OnUpdated += SetPartyData;
    }

    public void SetPartyData()
    {
        partyPoqimons = poqimonParty.Party;

        for (int i = 0; i < memberSlots.Length; i++)
        {
            if (i < partyPoqimons.Count)
            {
                memberSlots[i].gameObject.SetActive(true);
                memberSlots[i].SetData(partyPoqimons[i]);
            }
            else
                memberSlots[i].gameObject.SetActive(false);
        }

        UpdateMemberSelection(selection);

        messageTxt.text = "Choose a Pokemon";
    }

    public void HandleUpdate(Action onSelected, Action onBack)
    {
        var prevSelection = selection;

        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++selection;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --selection;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            selection += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            selection -= 2;

        selection = Mathf.Clamp(selection, 0, partyPoqimons.Count - 1);

        if (selection != prevSelection)
            UpdateMemberSelection(selection);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            onSelected?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            onBack?.Invoke();
        }
    }

    public void SetMessageText(string message)
    {
        messageTxt.text = message;
    }

    public void UpdateMemberSelection(int selectedMember)
    {
        for (int i = 0; i < partyPoqimons.Count; i++)
        {
            if (i == selectedMember)
                memberSlots[i].SetSelected(true);
            else
                memberSlots[i].SetSelected(false);
        }
    }
}
