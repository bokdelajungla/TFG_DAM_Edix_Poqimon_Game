using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] PoqimonBaseObject _base;
    [SerializeField] int lvl;
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

    public void SetUp()
    {
        Poquimon = new Poqimon(_base, lvl);
        
        if (isPLayer)
        {
            image.sprite = Poquimon.PoqimonBase.PoqimonBackSprite;
        }
        else 
        {

        }
        
        image.color = originalColor;
        //TODO: PlayEnterAnimation();

    }
}
