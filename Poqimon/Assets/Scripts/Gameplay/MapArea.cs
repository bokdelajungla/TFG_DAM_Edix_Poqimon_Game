using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{   
    [SerializeField] List<Poqimon> wildPoqimons;

    public Poqimon GetRandomWildPoqimon()
    {
        var wildPoqimon = wildPoqimons[Random.Range(0,wildPoqimons.Count)];
        wildPoqimon.Init();
        /*
        var wildParty = new PoqimonParty();
        wildParty.Party.Add(wildPoqimon);
        wildParty.Start();
        */
        return wildPoqimon;
    }
}
