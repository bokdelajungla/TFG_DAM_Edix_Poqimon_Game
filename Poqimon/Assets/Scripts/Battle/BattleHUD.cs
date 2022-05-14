using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] Text nameTxt;
    [SerializeField] Text lvlTxt;
    [SerializeField] HPBar hpBar;

    private Poqimon _poqimon;

    public void SetData(Poqimon poqimon)
    {
        _poqimon = poqimon;
        nameTxt.text = _poqimon.PoqimonBase.PoqimonName;
        lvlTxt.text = "lvl " + _poqimon.PoqimonLevel;
        hpBar.SetHP((float) _poqimon.CurrentHp / _poqimon.MaxHP);
        hpBar.setHPText(_poqimon.CurrentHp + " / " + _poqimon.MaxHP);
    }

    public IEnumerator UpdateHP()
    {
        yield return hpBar.SetHpSmooth((float) _poqimon.CurrentHp / _poqimon.MaxHP);
        hpBar.setHPText(_poqimon.CurrentHp + " / " + _poqimon.MaxHP);
    }
}
