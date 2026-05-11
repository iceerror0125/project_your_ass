using System;
using DialogueModule.Effect;
using TMPro;
using UnityEngine;

namespace DialogueModule.UI
{
    public class UITextBox : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _messageBox;
        [SerializeField] private TypeWriterEffect _effect;

        private string _cacheMessage;
        
        private void Awake()
        {
            _effect = new TypeWriterEffect(_messageBox);
        }

        public void SetMessage(string message, Action callback = null)
        {
            message ??= "";
            
            _messageBox.text = message;
            _cacheMessage = message;
            _effect.DoEffect(message);
            
            _effect.onEffectCompleted = callback;
        }

        public void SkipMessage()
        {
            _effect.SetTextInstantly(_cacheMessage);
        }
    }
}