using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MoveSelectionUI : MonoBehaviour
{
    [SerializeField] List<Text> moveTexts;
    [SerializeField] Color highlightColor;

    int currentSelection = 0;

    public void SetMoveData(List<MoveBase> currentMoves, MoveBase learnableMove)
    {
        for (int i=0; i<currentMoves.Count;i++)
        {
            moveTexts[i].text = currentMoves[i].MoveName;
        }

        moveTexts[currentMoves.Count].text = learnableMove.MoveName;
    }

    public void HandleMoveSelectionUI(Action<int> OnSelected)
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
            ++currentSelection;
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentSelection;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            --currentSelection;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentSelection;

        currentSelection = Mathf.Clamp(currentSelection, 0, PoqimonBase.MaxNumberOfMoves);

        UpdateMoveSelection(currentSelection);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            OnSelected?.Invoke(currentSelection);
        }
    }

    private void UpdateMoveSelection(int selection)
    {
        for (int i=0; i<PoqimonBase.MaxNumberOfMoves+1;i++)
        {
            if (i == selection)
            {
                moveTexts[i].color = highlightColor;
            }
            else
            {
                moveTexts[i].color = Color.black;
            }
        }    
    }
}
