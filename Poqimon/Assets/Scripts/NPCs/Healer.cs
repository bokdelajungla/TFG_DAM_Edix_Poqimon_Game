using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : MonoBehaviour
{

    [SerializeField] GameObject player;
    public IEnumerator dialog(Dialog dialog) {
        yield return DialogController.Instance.ShowDialog(dialog);
    }

    public void Heal() {

        var playerParty = player.GetComponent<PoqimonParty>();
        playerParty.Party.ForEach(p => p.Heal());
        playerParty.PartyUpdated();
    }
}
