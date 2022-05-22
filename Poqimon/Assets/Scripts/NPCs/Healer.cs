using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : MonoBehaviour
{
    [SerializeField] AudioClip healChime;

    public IEnumerator Heal(Transform player)
    {
        yield return DialogController.Instance.ShowDialogText($"Your poqimons look tired! Let me take care of them");

        AudioManager.i.PlaySfx(healChime, true);
        var playerParty = player.GetComponent<PoqimonParty>();
        playerParty.Party.ForEach(p => p.Heal());
        playerParty.PartyUpdated();

        yield return DialogController.Instance.ShowDialogText($"Your poqimon should be fully healed now");
    }
        
}