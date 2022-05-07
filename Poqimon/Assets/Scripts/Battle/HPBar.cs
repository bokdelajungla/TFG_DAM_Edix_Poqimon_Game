using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField] GameObject health;
    [SerializeField] Text healthTxt;
    [SerializeField] Color goodCondition;
    [SerializeField] Color averageCondition;
    [SerializeField] Color criticalCondition;
    
    public void SetHP(float hpNormalized)
    {
        health.transform.localScale = new Vector3(hpNormalized, 1f, 1f);
        SetHpColor(hpNormalized);
    }

    public void setHPText(string txt)
    {
        healthTxt.text = txt;
    }

    private void SetHpColor(float hpNormalized)
    {
        if (hpNormalized < 0.25f)
            health.GetComponent<Image>().color = criticalCondition;
        else if (hpNormalized < 0.5f)
            health.GetComponent<Image>().color = averageCondition;
        else
            health.GetComponent<Image>().color = goodCondition;
    }
}
