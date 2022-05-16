using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PoqimonParty : MonoBehaviour
{
    [SerializeField] List<Poqimon> party;

    public event Action OnUpdated;

    public List<Poqimon> Party
    {
        get
        {
            return party;
        }
        set
        {
            party = value;
            OnUpdated?.Invoke();
        }
    }

    public void Awake()
    {
        foreach (var poqimon in party)
        {
            poqimon.Init();
        }
    }

    private void Start() {
        
    }

    //Return the first not Fainted Pokemon in the party
    public Poqimon GetHealthyPoqimon()
    {   
        //Where + lambda(x is each of the element of the list) + First
        return party.Where(x => x.CurrentHp > 0).FirstOrDefault();
    }

    public void AddPoqimon(Poqimon newPoqimon)
    {
        if (party.Count < 6)
        {
            party.Add(newPoqimon);
            OnUpdated?.Invoke();
        }
        else 
        {
            //TODO: Add to storage PC
        }
    }

    public IEnumerator CheckForEvolutions() 
    {
        foreach (var poqimon in party)
        {
            var evolution = poqimon.CheckForEvolution();
            if (evolution != null)
            {
               yield return EvolutionController.i.Evolve(poqimon, evolution);
            }
        }

        OnUpdated?.Invoke();
    }

    public void PartyUpdated() {
        OnUpdated.Invoke();
    }

}
