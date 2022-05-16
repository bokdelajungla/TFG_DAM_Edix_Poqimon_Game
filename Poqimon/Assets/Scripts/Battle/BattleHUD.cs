using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] private Text nameTxt;
    [SerializeField] private Text lvlTxt;
    [SerializeField] private Text statusTxt;
    [SerializeField] private HPBar hpBar;
    [SerializeField] private GameObject expBar;
    
    private Dictionary<ConditionID, Color> statusColors;

    private Poqimon poqimon;

    public void SetData(Poqimon poqimon)
    {
        this.poqimon = poqimon;
        
        nameTxt.text = this.poqimon.PoqimonBase.PoqimonName;
        SetLvl();
        hpBar.SetHP((float) this.poqimon.CurrentHp / this.poqimon.MaxHp);
        hpBar.setHPText(this.poqimon.CurrentHp + " / " + this.poqimon.MaxHp);
        SetExp();

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

    public void SetExp()
    {
        //Only for playerHud
        if (expBar != null)
        {
            float normalizedExp = GetNormalizedExp();
            expBar.transform.localScale = new Vector3 (normalizedExp, 1f, 1f);
        }
    }

    public IEnumerator SetExpSmooth(bool reset=false)
    {
        //Only for playerHud
        if (expBar != null)
        {
            if (reset) { expBar.transform.localScale = new Vector3 (0, 1f, 1f); }
            float normalizedExp = GetNormalizedExp();
            yield return expBar.transform.DOScaleX(normalizedExp, 1.5f).WaitForCompletion();
        }

    }

    float GetNormalizedExp()
    {
        int currentLevelExp = poqimon.PoqimonBase.GetExperienceForLvl(poqimon.PoqimonLevel);
        int nextLevelExp = poqimon.PoqimonBase.GetExperienceForLvl(poqimon.PoqimonLevel + 1);
        float normalizedExp = (float)(poqimon.Exp - currentLevelExp) / (nextLevelExp - currentLevelExp);
        return Mathf.Clamp01(normalizedExp);
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

    public void SetLvl()
    {
        lvlTxt.text = "lvl " + poqimon.PoqimonLevel;
    }
}
