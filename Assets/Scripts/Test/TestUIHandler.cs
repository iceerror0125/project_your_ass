using System;
using DialogueModule;
using DialogueModule.UI;
using ProtectYourAss.System;
using UnityEngine;

namespace Test
{
    public class TestUIHandler : MonoBehaviour
    {
        [SerializeField] private UIDialogue _ui;
        private void OnEnable()
        {
            ObserverSystem.AddListener(ObserveMessage.ActivateDialogue, StartDialogue);
        }

        private void OnDisable()
        {
            ObserverSystem.RemoveListener(ObserveMessage.ActivateDialogue, StartDialogue);
        }

        private void StartDialogue(object message)
        {
            if (!_ui.gameObject.activeSelf)
            {
                _ui.gameObject.SetActive(true);
            }
            _ui.StartDialogue();
        }
        
        
    }
}