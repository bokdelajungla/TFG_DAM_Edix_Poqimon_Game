using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] bool isPLayer;
    
    public Poqimon Poqimon { get; set; }

    Image image;
    Vector3 originalPosition;
    Color originalColor;

    private void Awake()
    {
        image = GetComponent<Image>();
        originalPosition = image.transform.localPosition;
        originalColor = image.color;
    }

    public void SetUp(Poqimon poqimon)
    {
        Poqimon = poqimon;
        
        if (isPLayer)
        {
            image.sprite = Poqimon.PoqimonBase.PoqimonBackSprite;
        }
        else 
        {
            image.sprite = Poqimon.PoqimonBase.PoqimonFrontSprite;
        }
        
        image.color = originalColor;
        //TODO: PlayEnterAnimation();

    }
}
