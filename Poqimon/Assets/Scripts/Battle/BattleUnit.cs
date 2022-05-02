using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] PoqimonBaseObject _base;
    [SerializeField] int lvl;
    [SerializeField] bool isPLayer;
    
    public Poqimon Poquimon { get; set; }

    public void setUp()
    {
        Poquimon = new Poqimon(_base, lvl);
        if (isPLayer)
        {
            GetComponent<Image>().sprite = Poquimon.PoqimonBase.PoqimonBackSprite;
        }
    }
}
