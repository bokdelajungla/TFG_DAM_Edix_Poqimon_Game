using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] private Text nameTxt;
    [SerializeField] private Text lvlTxt;
    [SerializeField] private Text statusTxt;
    [SerializeField] private HPBar hpBar;
    
    private Dictionary<ConditionID, Color> statusColors;

    private Poqimon poqimon;

    public void SetData(Poqimon poqimon)
    {
        this.poqimon = poqimon;
        
        nameTxt.text = this.poqimon.PoqimonBase.PoqimonName;
        lvlTxt.text = "lvl " + this.poqimon.PoqimonLevel;
        hpBar.SetHP((float) this.poqimon.CurrentHp / this.poqimon.MaxHp);
        hpBar.setHPText(this.poqimon.CurrentHp + " / " + this.poqimon.MaxHp);

        statusColors = new Dictionary<ConditionID, Color>()
        {
            { ConditionID.psn, new Color(57, 29, 194) },
            { ConditionID.brn, new Color(241, 80, 0) },
            { ConditionID.slp, new Color(145, 145, 145) },
            { ConditionID.par, new Color(147, 122, 0) },
            { ConditionID.frz, new Color(0, 212, 241) }
        };

        SetStatusTxt();
        poqimon.OnStatusChanged += SetStatusTxt;
    }

    private void SetStatusTxt()
    {
        if (poqimon.Status == null)
        {
            statusTxt.text = "";
        }
        else
        {
            statusTxt.text = poqimon.Status.Id.ToString().ToUpper();
            statusTxt.color = statusColors[poqimon.Status.Id];
        }
    }

    public IEnumerator UpdateHP()
    {
        if (poqimon.HpChanged)
        {
            yield return hpBar.SetHpSmooth((float) poqimon.CurrentHp / poqimon.MaxHp);
            // TODO SetTxtSmooth - funcion propia
            hpBar.setHPText(poqimon.CurrentHp + " / " + poqimon.MaxHp);
            poqimon.HpChanged = false;
        }
        
        yield return hpBar.SetHpSmooth((float) poqimon.CurrentHp / poqimon.MaxHp);
        hpBar.setHPText(poqimon.CurrentHp + " / " + poqimon.MaxHp);
    }
}
