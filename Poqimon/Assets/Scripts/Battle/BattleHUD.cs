using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] Text nameTxt;
    [SerializeField] Text lvlTxt;
    [SerializeField] HPBar hpBar;

    private Poqimon poqimon;

    public void SetData(Poqimon poqimon)
    {
        this.poqimon = poqimon;
        nameTxt.text = this.poqimon.PoqimonBase.PoqimonName;
        lvlTxt.text = "lvl " + this.poqimon.PoqimonLevel;
        hpBar.SetHP((float) this.poqimon.CurrentHp / this.poqimon.MaxHp);
        hpBar.setHPText(this.poqimon.CurrentHp + " / " + this.poqimon.MaxHp);
    }

    public IEnumerator UpdateHP()
    {
        yield return hpBar.SetHpSmooth((float) poqimon.CurrentHp / poqimon.MaxHp);
        hpBar.setHPText(poqimon.CurrentHp + " / " + poqimon.MaxHp);
    }
}
