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
    Animator animator;
    
    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void SetData(Poqimon poqimon)
    {
        this.poqimon = poqimon;
        UpdateData();

        poqimon.OnHPChanged += UpdateData;
    }

    void UpdateData()
    {
        nameTxt.text = poqimon.PoqimonBase.PoqimonName;
        lvlTxt.text = poqimon.PoqimonLevel.ToString();
        hpBar.SetHP((float) poqimon.CurrentHp / poqimon.MaxHp);
        hpBar.setHPText(poqimon.CurrentHp.ToString());
        hpBar.setMaxHpText(poqimon.MaxHp.ToString());
    }

    public void SetSelected(bool selected)
    {
        if(animator != null) {
            if(selected)
                animator.SetBool("isSelected", true);
            else
                animator.SetBool("isSelected", false);
        }
    }
}
