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
    
    [SerializeField] Color psnColor;
    [SerializeField] Color brnColor;
    [SerializeField] Color slpColor;
    [SerializeField] Color parColor;
    [SerializeField] Color frzColor;

    private Dictionary<ConditionID, Color> statusColors;

    private Poqimon poqimon;

    public void SetData(Poqimon poqimon)
    {
        if (this.poqimon != null)
        {
            this.poqimon.OnHPChanged -= UpdateHP;
            this.poqimon.OnStatusChanged -= SetStatusTxt;
        }
        this.poqimon = poqimon;

        nameTxt.text = poqimon.PoqimonBase.PoqimonName;
        SetLvl();
        hpBar.SetHP((float) poqimon.CurrentHp / poqimon.MaxHp);
        hpBar.setHPText(poqimon.CurrentHp + " / " + poqimon.MaxHp);
        SetExp();

        statusColors = new Dictionary<ConditionID, Color>()
        {
            { ConditionID.psn, psnColor },
            { ConditionID.brn, brnColor },
            { ConditionID.slp, slpColor },
            { ConditionID.par, parColor },
            { ConditionID.frz, frzColor }
        };

        SetStatusTxt();
        poqimon.OnHPChanged += UpdateHP;
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

    public void SetLvl()
    {
        lvlTxt.text = "lvl " + poqimon.PoqimonLevel;
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

    public void UpdateHP()
    {
        StartCoroutine(UpdateHPAsync());
    }

    public IEnumerator UpdateHPAsync()
    {
        yield return hpBar.SetHpSmooth((float) poqimon.CurrentHp / poqimon.MaxHp);
            // TODO SetTxtSmooth - funcion propia
            hpBar.setHPText(poqimon.CurrentHp + " / " + poqimon.MaxHp);
            poqimon.HpChanged = false;
    }

    public IEnumerator WaitForHPUpdate()
    {
        yield return new WaitUntil(() => hpBar.IsUpdating == false);
    }

    public void ClearData()
    {
        if (poqimon != null)
        {
            poqimon.OnHPChanged -= UpdateHP;
            poqimon.OnStatusChanged -= SetStatusTxt;
        }
    }

}
