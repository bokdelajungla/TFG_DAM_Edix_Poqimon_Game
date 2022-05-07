using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] Text nameTxt;
    [SerializeField] Text lvlTxt;
    [SerializeField] HPBar hpBar;

    Poqimon poqimon;

    public void SetData(Poqimon poqimon)
    {
        nameTxt.text = poqimon.PoqimonBase.PoqimonName;
        lvlTxt.text = "lvl " + poqimon.PoqimonLevel;
        hpBar.SetHP((float) poqimon.CurrentHp / poqimon.MaxHP);
        hpBar.setHPText(poqimon.CurrentHp + " / " + poqimon.MaxHP);
    }
}
