using System;
using System.Collections.Generic;
using DialogueModule.Model;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DialogueModule.UI
{
    public sealed class UIDialogue : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private GameObject _root;

        [Header("Characters")]
        [SerializeField] private UICharacter _leftCharacter;
        [SerializeField] private UICharacter _rightCharacter;

        [Header("Content")]
        [SerializeField] private List<UIOption> _options;
        [SerializeField] private UITextBox _textbox;

        private UICharacterRegister _characterRegister;

        public event Action BackgroundPressed;
        public event Action<DialogueChoice> OptionSelected;

        private void Awake()
        {
            _characterRegister = new UICharacterRegister(_leftCharacter, _rightCharacter);
            foreach (UIOption option in _options)
            {
                option.Selected += HandleOptionSelected;
                option.Hide();
            }
        }

        private void OnDestroy()
        {
            foreach (UIOption option in _options)
            {
                if (option != null)
                {
                    option.Selected -= HandleOptionSelected;
                }
            }
        }

        public void SetVisible(bool visible)
        {
            if (!visible)
            {
                HideOptions();
                _characterRegister.Clear();
            }

            _root.SetActive(visible);
        }

        public void Render(DialogueData data)
        {
            if (data == null || data.IsComplete)
            {
                throw new ArgumentException("Cannot render completed dialogue data.", nameof(data));
            }

            _characterRegister.Present(data.Speaker, data.Side);
            _textbox.SetMessage(data.Message);
            RenderOptions(data.Choices);
        }

        public bool TrySkipTyping()
        {
            return _textbox.TrySkipTyping();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            GameObject pointerTarget = eventData.pointerCurrentRaycast.gameObject;
            if (pointerTarget != null && pointerTarget.GetComponentInParent<UIOption>() != null)
            {
                return;
            }

            BackgroundPressed?.Invoke();
        }

        private void RenderOptions(IReadOnlyList<DialogueChoice> choices)
        {
            HideOptions();

            int visibleChoiceCount = Math.Min(choices.Count, _options.Count);
            for (int index = 0; index < visibleChoiceCount; index++)
            {
                _options[index].Show(choices[index]);
            }

            if (choices.Count > _options.Count)
            {
                Debug.LogWarning(
                    $"Dialogue has {choices.Count} choices but the UI only has {_options.Count} option slots.",
                    this);
            }
        }

        private void HideOptions()
        {
            foreach (UIOption option in _options)
            {
                option.Hide();
            }
        }

        private void HandleOptionSelected(DialogueChoice choice)
        {
            OptionSelected?.Invoke(choice);
        }
    }
}
