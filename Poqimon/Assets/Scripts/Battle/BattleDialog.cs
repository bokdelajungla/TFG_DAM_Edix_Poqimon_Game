using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BattleDialog : MonoBehaviour
{
    [SerializeField] int lettersPerSecond;
    [SerializeField] Color highlightedColor;
    
    [SerializeField] Text dialogTxt;
    
    [SerializeField] private GameObject actionSelector;
    [SerializeField] private GameObject moveSelector;
    [SerializeField] private GameObject moveDetails;

    [SerializeField] List<Text> actionsTxts;
    [SerializeField] List<Text> movesTxts;
    
    [SerializeField] Text ppTxt;
    [SerializeField] Text moveTypeTxt;
    [SerializeField] Text moveCategoryTxt;

    public void setTxt(string txt)
    {
        dialogTxt.text = txt;
    }
    
    public IEnumerator TypeTxt(string txt) {
        dialogTxt.text = "";
        foreach (var letter in txt.ToCharArray())
        {
            dialogTxt.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
    }

    public void EnableDialogTxt(bool enabled)
    {
        dialogTxt.enabled = enabled;
    }

    public void EnableActionSelector(bool enabled)
    {
        actionSelector.SetActive(enabled);
    }
    
    public void EnableMoveSelector(bool enabled)
    {
        moveSelector.SetActive(enabled);
        moveDetails.SetActive(enabled);
    }

    public void UpdateActionSelection(int selectedAction)
    {
        for (int i = 0; i < actionsTxts.Count; i++)
        {
            if (i == selectedAction)
            {
                actionsTxts[i].color = highlightedColor;
            }
            else
            {
                actionsTxts[i].color = Color.black;
            }
        }
    }

    public void UpdateMoveSelection(int selectedMove)
    {
        for (int i = 0; i < movesTxts.Count; i++)
        {
            if (i == selectedMove)
            {
                movesTxts[i].color = highlightedColor;
            }
            else
            {
                movesTxts[i].color = Color.black;
            }
        }
    }
}
