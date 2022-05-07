using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] bool isPLayer;
    
    public Poqimon Poquimon { get; set; }

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
        Poquimon = poqimon;
        
        if (isPLayer)
        {
            image.sprite = Poquimon.PoqimonBase.PoqimonBackSprite;
        }
        else 
        {
            image.sprite = Poquimon.PoqimonBase.PoqimonFrontSprite;
        }
        
        image.color = originalColor;
        //TODO: PlayEnterAnimation();

    }
}
