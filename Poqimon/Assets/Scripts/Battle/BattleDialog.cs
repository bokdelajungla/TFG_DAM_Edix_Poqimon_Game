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
            actionsTxts[i].color = (i == selectedAction) ? highlightedColor : Color.black;
        }
    }

    public void UpdateMoveSelection(int selectedMove, Move move)
    {
        for (int i = 0; i < movesTxts.Count; i++)
        {
            movesTxts[i].color = (i == selectedMove) ? highlightedColor : Color.white;
        }

        ppTxt.text = $"PP {move.MovePP}/{move.MoveBase.MovePP}";
        moveTypeTxt.text = move.MoveBase.MoveType.ToString();
    }

    public void SetMoveNames(List<Move> moves)
    {
        for (int i = 0; i < movesTxts.Count; i++)
        {
            movesTxts[i].text = (i < moves.Count) ? moves[i].MoveBase.MoveName : movesTxts[i].text = "--";
        }
    }
}
