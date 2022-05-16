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
        yield return new WaitForSeconds(1f);
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
        ppTxt.color = (move.MovePP == 0) ? Color.red : Color.black;
        moveTypeTxt.text = move.MoveBase.MoveType.ToString();
        switch (moveTypeTxt.text)
        {
            case "Fire":
                moveTypeTxt.color = Color.red;
                break;
            case "Normal":
                moveTypeTxt.color = Color.gray;
                break;
            case "None":
                moveTypeTxt.color = Color.black;
                break;
            case "Dark":
                moveTypeTxt.color = new Color(  91, 72, 23  );
                break;
            case "Fairy":
                moveTypeTxt.color = new Color(  201, 144, 198  );
                break;
            case "Ground":
                moveTypeTxt.color = new Color(  190, 170, 72  );
                break;
            case "Flying":
                moveTypeTxt.color = new Color(  133, 115, 238  );
                break;
            case "Bug":
                moveTypeTxt.color = new Color( 138, 202, 0 );
                break;
            case "Rock":
                moveTypeTxt.color = new Color( 126, 84, 0 );
                break;
            case "Ghost":
                moveTypeTxt.color = new Color( 73, 0, 162 );
                break;
            case "Poison":
                moveTypeTxt.color = new Color( 163, 0, 236 );
                break;
            case "Fight":
                moveTypeTxt.color = new Color(  150, 0, 0  );
                break;
            case "Ice":
                moveTypeTxt.color = Color.cyan;
                break;
            case "Electr":
                moveTypeTxt.color = Color.yellow;
                break;
            case "Grass":
                moveTypeTxt.color = Color.green;
                break;
            case "Dragon":
                moveTypeTxt.color = Color.blue;
                break;
            case "Water":
                moveTypeTxt.color = new Color( 0, 183, 241 );
                break;
            case "Psychc":
                moveTypeTxt.color = Color.magenta;
                break;
            case "Steel":
                moveTypeTxt.color = Color.grey;
                break;
            default:
                moveTypeTxt.color = Color.black;
                break;
        }
        moveCategoryTxt.text = move.MoveBase.MoveCategory.ToString();
        
    }

    public void SetMoveNames(List<Move> moves)
    {
        for (int i = 0; i < movesTxts.Count; i++)
        {
            movesTxts[i].text = (i < moves.Count) ? moves[i].MoveBase.MoveName : movesTxts[i].text = "--";
        }
    }
}
