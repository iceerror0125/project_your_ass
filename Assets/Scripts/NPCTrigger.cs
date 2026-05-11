using System;
using ProtectYourAss.System;
using UnityEngine;

public class NPCTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (PlayerUtils.IsPlayer(other))
        {
            Debug.Log("NPCTrigger");
            ObserverSystem.PostEvent(ObserveMessage.ActivateDialogue);
        }
    }
}
