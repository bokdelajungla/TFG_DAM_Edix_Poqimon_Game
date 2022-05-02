using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] Text nameTxt;
    [SerializeField] Text lvlTxt;
    [SerializeField] HPBar hpBar;

    public void SetData(Poqimon poquimon)
    {
        nameTxt.text = poquimon.PoqimonBase.name;
        lvlTxt.text = "lvl " + poquimon.PoqimonLevel;
        hpBar.SetHP((float) poquimon.CurrentHp / poquimon.MaxHP);
        hpBar.setHPText(poquimon.CurrentHp + " / " + poquimon.MaxHP);
    }
}
