using EffectModule;
using TMPro;
using UnityEngine;

namespace DialogueModule.UI
{
    public sealed class UITextBox : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _messageBox;

        private TypeWriterEffect _effect;
        private string _currentMessage = string.Empty;
        private bool _isTyping;

        private void Awake()
        {
            _effect = new TypeWriterEffect(_messageBox);
        }

        private void OnDisable()
        {
            TrySkipTyping();
        }

        public void SetMessage(string message)
        {
            TrySkipTyping();
            _currentMessage = message ?? string.Empty;

            if (_currentMessage.Length == 0)
            {
                _messageBox.text = string.Empty;
                _messageBox.maxVisibleCharacters = 0;
                return;
            }

            _isTyping = true;
            _effect
                .SetText(_currentMessage)
                .SetCallback(HandleTypingCompleted)
                .DoEffect();
        }

        public bool TrySkipTyping()
        {
            if (!_isTyping)
            {
                return false;
            }

            _effect.SetTextInstantly(_currentMessage);
            _isTyping = false;
            return true;
        }

        private void HandleTypingCompleted()
        {
            _isTyping = false;
        }
    }
}
