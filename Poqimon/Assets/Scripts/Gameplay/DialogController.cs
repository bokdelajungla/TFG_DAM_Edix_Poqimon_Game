using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogController : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] Text dialogText;
    [SerializeField] float lettersPerSecond;

    public event Action OnShowDialog;
    public event Action OnCloseDialog;

    public static DialogController Instance {get; private set;}

    private void Awake()
    {
        Instance = this;
    }

    //******//
    Dialog dialog;
    int currentLine = 0;
    bool isTyping;
    public bool IsShowing {get; private set;}

    public IEnumerator ShowDialog(Dialog dialog, Action OnFinished = null)
    {
        yield return new WaitForEndOfFrame();
        OnShowDialog?.Invoke();

        IsShowing = true;
        this.dialog = dialog;
        dialogBox.SetActive(true);
        StartCoroutine(TypeDialog(dialog.Lines[0]));
    }

    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Z) && !isTyping)
        {
            ++currentLine;
            if (currentLine < dialog.Lines.Count)
            {
                StartCoroutine(TypeDialog(dialog.Lines[currentLine]));
            }
            else
            {
                IsShowing = false;
                currentLine = 0;
                dialogBox.SetActive(false);
                OnCloseDialog?.Invoke();
            }
        }
    }

    //Coroutine for showing text letter by letter     
    public IEnumerator TypeDialog(string line)
    {
        isTyping = true;
        dialogText.text = "";
        foreach (var letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f/lettersPerSecond);
        }
        isTyping = false;
    } 

    //Simple Dialog Text for multiple Purpouses
    public IEnumerator ShowDialogText(String text, bool waitForInput=true)
    {
        IsShowing = true;
        dialogBox.SetActive(true);
        yield return TypeDialog(text);
        if (waitForInput){
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
        }
        
        dialogBox.SetActive(false);
        IsShowing = false;
    } 
}
