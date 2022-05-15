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
        hpBar.SetHP((float) _poqimon.CurrentHp / _poqimon.MaxHp);
        hpBar.setHPText(_poqimon.CurrentHp + " / " + _poqimon.MaxHp);
    }

    public IEnumerator UpdateHP()
    {
        if (_poqimon.HpChanged)
        {
            yield return hpBar.SetHpSmooth((float) _poqimon.CurrentHp / _poqimon.MaxHp);
            // TODO SetTxtSmooth - funcion propia
            hpBar.setHPText(_poqimon.CurrentHp + " / " + _poqimon.MaxHp);
            _poqimon.HpChanged = false;
        }
    }
}
