using System;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueModule.UI
{
    public class UIOption : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _message;
        [SerializeField] private Button _button;
        [SerializeField] private GameObject _backgroundObj;
        
        private Choice choice;
        private bool _isEnabled = false;
        
        public event Action<Choice> onClicked;
        
        private void OnEnable()
        {
            _button.onClick.AddListener(OnButtonClick);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            onClicked?.Invoke(choice);
        }

        public void SetChoice(Choice choice)
        {
            this.choice = choice;
        }
        
        public void SetText(string text)
        {
            this._message.text = text;
        }

        public bool IsEnabled()
        {
            return _isEnabled;
        }
        
        public void Enable()
        {
            _isEnabled = true;
            ShowText(true);
        }

        public void Disable()
        {
            _isEnabled = false;
            ShowText(false);
        }

        private void ShowText(bool enabled)
        {
            _message.gameObject.SetActive(enabled);
            _backgroundObj.SetActive(enabled);
        }
        
    }
}