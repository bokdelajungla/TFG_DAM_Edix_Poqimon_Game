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
    
    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void SetData(Poqimon poqimon)
    {
        this.poqimon = poqimon;
        nameTxt.text = poqimon.PoqimonBase.PoqimonName;
        lvlTxt.text = poqimon.PoqimonLevel.ToString();
        hpBar.SetHP((float) poqimon.CurrentHp / poqimon.MaxHP);
        hpBar.setHPText(poqimon.CurrentHp.ToString());
        hpBar.setMaxHpText(poqimon.MaxHP.ToString());
    }

    public void SetSelected(bool selected)
    {
        if(selected)
            animator.SetBool("isSelected", true);
        else
            animator.SetBool("isSelected", false);
    }
}
