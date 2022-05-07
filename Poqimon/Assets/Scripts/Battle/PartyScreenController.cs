using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreenController : MonoBehaviour
{
    [SerializeField] Text messageTxt;

    PartyMemberController[] memberSlots;

    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberController>();
    }

    public void SetPartyData(List<Poqimon> partyPoqimons)
    {
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
}
