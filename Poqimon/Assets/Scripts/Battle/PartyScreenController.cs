using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreenController : MonoBehaviour
{
    [SerializeField] Text messageTxt;

    PartyMemberController[] memberSlots;
    List<Poqimon> partyPoqimons;

    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberController>();
    }

    public void SetPartyData(List<Poqimon> partyPoqimons)
    {
        this.partyPoqimons = partyPoqimons;

        for (int i = 0; i<memberSlots.Length; i++)
        {
            if (i < partyPoqimons.Count)
            {
                memberSlots[i].SetData(partyPoqimons[i]);
            }
            else
            {
                memberSlots[i].gameObject.SetActive(false);
            }
        }

        messageTxt.text = "Choose a Poqimon";
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
