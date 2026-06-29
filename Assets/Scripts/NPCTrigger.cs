using System;
using UnityEngine;

public class NPCTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (PlayerUtils.IsPlayer(other))
        {
            Debug.Log("NPCTrigger");
            ObserverSystem.Announce(new ActivateDialogueMessage());
        }
    }
}
