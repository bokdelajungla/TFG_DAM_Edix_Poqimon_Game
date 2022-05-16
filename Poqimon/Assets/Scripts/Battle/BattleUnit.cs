using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] private bool isPLayer;
    public bool IsPlayer => isPLayer;

    [SerializeField] private BattleHUD hud;
    public BattleHUD Hud => hud;
    
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
        
        image.sprite = (isPLayer) ?  Poqimon.PoqimonBase.PoqimonBackSprite : Poqimon.PoqimonBase.PoqimonFrontSprite;

        hud.SetData(poqimon);
        
        image.color = originalColor;
        PlayEnterAnimation();
    }
    
    public void PlayEnterAnimation()
    {
        image.transform.localPosition = (isPLayer) ? new Vector3(-550f, originalPosition.y) :  new Vector3(550f, originalPosition.y);
        image.transform.DOLocalMoveX(originalPosition.x, 1f);
    }

    public void PlayAtkAnimation()
    {
        var sequence = DOTween.Sequence();
        if (isPLayer)
        {
            sequence.Append(image.transform.DOLocalMoveX(originalPosition.x + 50f, 0.2f));
        }
        else
        {
            sequence.Append(image.transform.DOLocalMoveX(originalPosition.x - 50f, 0.2f));
        }
        sequence.Append(image.transform.DOLocalMoveX(originalPosition.x, 0.2f));
    }

    public void PlayHitAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOColor(Color.gray, 0.1f));
        sequence.Append(image.DOColor(originalColor, 0.1f));
    }

    public void PlayFaintedAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.transform.DOLocalMoveY(originalPosition.y - 150f, 0.5f));
        sequence.Join(image.DOFade(0f, 0.5f));
    }

    public IEnumerator playCapturedAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOFade(0, 0.5f));
        sequence.Join(transform.DOLocalMoveY(originalPosition.y + 50f, 0.5f));
        sequence.Join(transform.DOScale(new Vector3(0.3f, 0.3f, 1f), 0.5f));
        yield return sequence.WaitForCompletion();
    }
    
}
