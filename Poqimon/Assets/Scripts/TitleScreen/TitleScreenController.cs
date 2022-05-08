using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    // Start is called before the first frame update
    void Start()
    {
        canvasAudioSource = GetComponent<AudioSource>();
        newGameButton.GetComponent<Button>().Select();
        backgroundStartPosition = background.transform.position;
        //TODO: SAVE GAME STATUS
        //if there is gamesaved -> Enable continue button
        //else
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
        SceneManager.LoadScene("World");
        
    }
    public void ContinueGame()
    {
        //TODO: ContinueGame Logic
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
