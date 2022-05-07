using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PoqimonParty : MonoBehaviour
{
    [SerializeField] List<Poqimon> party;
    public List<Poqimon> Party
    {
        get
        {
            return party;
        }
    }

    public void Start()
    {
        foreach (var poqimon in party)
        {
            poqimon.Init();
        }
    }

    //Return the first not Fainted Pokemon in the party
    public Poqimon GetHealthyPoqimon()
    {   
        //Where + lambda(x is each of the element of the list) + First
        return party.Where(x => x.CurrentHp > 0).FirstOrDefault();
    }

}
