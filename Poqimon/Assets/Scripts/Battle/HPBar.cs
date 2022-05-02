using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField] GameObject health;
    [SerializeField] Text healthTxt;
    
    public void SetHP(float hp)
    {
        health.transform.localScale = new Vector3(hp / 1.2f, 0.7f);
    }

    public void setHPText(string txt)
    {
        healthTxt.text = txt;
    }
}
