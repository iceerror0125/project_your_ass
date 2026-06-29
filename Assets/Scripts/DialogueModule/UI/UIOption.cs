using System;
using DialogueModule.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueModule.UI
{
    public sealed class UIOption : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _message;
        [SerializeField] private Button _button;
        [SerializeField] private GameObject _backgroundObj;

        private DialogueChoice _choice;

        public event Action<DialogueChoice> Selected;

        private void OnEnable()
        {
            _button.onClick.AddListener(HandleClick);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(HandleClick);
        }

        public void Show(DialogueChoice choice)
        {
            _choice = choice;
            _message.text = choice.Text;
            _message.gameObject.SetActive(true);
            _backgroundObj.SetActive(true);
            _button.interactable = true;
        }

        public void Hide()
        {
            _choice = default;
            _message.text = string.Empty;
            _message.gameObject.SetActive(false);
            _backgroundObj.SetActive(false);
            _button.interactable = false;
        }

        private void HandleClick()
        {
            if (_button.interactable)
            {
                Selected?.Invoke(_choice);
            }
        }
    }
}
