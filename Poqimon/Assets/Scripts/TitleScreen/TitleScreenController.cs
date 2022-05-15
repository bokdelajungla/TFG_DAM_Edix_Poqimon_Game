using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class TitleScreenController : MonoBehaviour
{
    public AudioClip buttonSelected;
    public GameObject newGameButton;
    public GameObject continueButton;
    public GameObject exitButton;
    public GameObject background;    
    
    private AudioSource canvasAudioSource;
    private float delay;
    private Vector3 backgroundStartPosition;
    private string saveFile = "saveSlot1";

    // Start is called before the first frame update
    void Start()
    {
        canvasAudioSource = GetComponent<AudioSource>();
        newGameButton.GetComponent<Button>().Select();
        backgroundStartPosition = background.transform.position;
        //TODO: SAVE GAME STATUS
        //if there is gamesaved -> Enable continue button
        var path = SavingSystem.i.GetPath(saveFile);
        Debug.Log(path);
        if (File.Exists(path))
            continueButton.GetComponent<Button>().interactable = true;
        else
            continueButton.GetComponent<Button>().interactable = false;
    }

    // Update is called once per frame
    void Update()
    {
        AnimateBackground();

        if(Input.GetKeyDown(KeyCode.Z))
        {
            if (newGameButton.GetComponent<ButtonSelectionTracker>().IsSelected)
                StartGame();
            else if (continueButton.GetComponent<ButtonSelectionTracker>().IsSelected)
                ContinueGame();
            else if (exitButton.GetComponent<ButtonSelectionTracker>().IsSelected)
                QuitGame();
        }
    }

    public void OnMouseOverButton(){
        canvasAudioSource.PlayOneShot(buttonSelected);
    }
    public void StartGame(){
        canvasAudioSource.PlayOneShot(buttonSelected);
        SavingSystem.i.IsNewGame = true;
        SceneManager.LoadScene("World");
        
    }
    public void ContinueGame()
    {
        //TODO: ContinueGame Logic
        canvasAudioSource.PlayOneShot(buttonSelected);
        SavingSystem.i.IsNewGame = false;
        SceneManager.LoadScene("World");
        
    }
    public void QuitGame(){
        canvasAudioSource.PlayOneShot(buttonSelected);
        Application.Quit();
    }

    void AnimateBackground()
    {
        background.transform.position -= new Vector3(1f,0f,0f);
        if (background.transform.position.x <= -359f)
        {
            background.transform.position = backgroundStartPosition;
        }
    }  

    IEnumerator ChangeSceneDelay(string SceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneName);  
    }

}
