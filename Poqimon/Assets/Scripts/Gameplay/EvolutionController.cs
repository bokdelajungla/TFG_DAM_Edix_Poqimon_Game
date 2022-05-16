using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvolutionController : MonoBehaviour
{
    [SerializeField] GameObject evolutionUI;
    [SerializeField] GameObject originalPoqimon;
    [SerializeField] GameObject evolutionPoqimon;
    
    [SerializeField] GameObject evolutionSwirl;
    [SerializeField] GameObject evolutionBurst;
    
    [SerializeField] private AudioClip evolutionMusic;
    public event Action OnEvolutionStart;
    public event Action OnEvolutionEnd;

    [SerializeField] float animationDuration;

    Image poqimonImage;
    Image evolutionImage;

    //Singleton Instance
    public static EvolutionController i {get; private set;}
    
    private void Awake() 
    {
        i = this;
        poqimonImage = originalPoqimon.GetComponent<Image>();
        evolutionImage = evolutionPoqimon.GetComponent<Image>();   
    }

    public IEnumerator Evolve(Poqimon poqimon, Evolution evolution)
    {
        OnEvolutionStart?.Invoke();
        evolutionUI.SetActive(true);

        AudioManager.i.PlayMusic(evolutionMusic);
        
        poqimonImage.sprite = poqimon.PoqimonBase.PoqimonFrontSprite;
        evolutionImage.sprite = evolution.EvolvesInto.PoqimonFrontSprite;

        yield return DialogController.Instance.ShowDialogText($"What?! Your {poqimon.PoqimonBase.PoqimonName} is evolving!");

        StartCoroutine(EvolveAnimation(poqimonImage, evolutionImage));
        yield return new WaitForSeconds(animationDuration);
        
        
        var originalName = poqimon.PoqimonBase.PoqimonName;
        poqimon.Evolve(evolution);
        poqimonImage.sprite = poqimon.PoqimonBase.PoqimonFrontSprite;
        poqimonImage.color = Color.white;
        evolutionPoqimon.SetActive(false);
        originalPoqimon.transform.localScale = new Vector3(1f,1f,1f);
        originalPoqimon.SetActive(true);  
        
        yield return DialogController.Instance.ShowDialogText($"Your {originalName} evolved into {poqimon.PoqimonBase.PoqimonName}!");

        evolutionUI.SetActive(false);
        OnEvolutionEnd?.Invoke();
    }

    public IEnumerator EvolveAnimation(Image poqimonImage, Image evolutionImage)
    {
        poqimonImage.color = Color.black;
        evolutionImage.color = Color.black;
        evolutionImage.transform.localScale = new Vector3 (0f, 0f, 0f);
        evolutionPoqimon.SetActive(true);
        evolutionSwirl.SetActive(true);

        //TODO: Implement Transition Animation
        StartCoroutine(EvolveShrink(originalPoqimon));

        yield return new WaitForSeconds(animationDuration*4/6);
        
        evolutionSwirl.SetActive(false);
        evolutionBurst.SetActive(true);
        evolutionImage.transform.localScale = new Vector3 (1f, 1f, 1f);
        originalPoqimon.SetActive(false);
        evolutionPoqimon.SetActive(true);
        
        yield return new WaitForSeconds(animationDuration*2/6);

        evolutionBurst.SetActive(false);
    }

    public IEnumerator EvolveShrink(GameObject poqimonImage)
    {
        var elapsedTime = 0f;
        while(elapsedTime < animationDuration*4/6)
        {
            var t = elapsedTime * 6/(4*animationDuration);
            poqimonImage.transform.localScale = new Vector3 (1f-t,1f-t,1f-t);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    public IEnumerator EvolveGrowth(GameObject poqimonImage)
    {
        yield return null;
    }
}
