using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberController : MonoBehaviour
{
    [SerializeField] Text nameTxt;
    [SerializeField] Text lvlTxt;
    [SerializeField] PartyHpBar hpBar;

    Poqimon poqimon;

    public void SetData(Poqimon poqimon)
    {
        this.poqimon = poqimon;
        nameTxt.text = poqimon.PoqimonBase.PoqimonName;
        lvlTxt.text = poqimon.PoqimonLevel.ToString();
        hpBar.SetHP((float) poqimon.CurrentHp / poqimon.MaxHP);
        hpBar.setHPText(poqimon.CurrentHp.ToString());
        hpBar.setMaxHpText(poqimon.MaxHP.ToString());
    }
}
