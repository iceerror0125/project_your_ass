using DialogueModule.Model;
using EffectModule;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueModule.UI
{
    public sealed class UICharacter : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private ImageFadeAndMovementEffect _effect;

        private bool _isVisible;

        public void SetCharacter(Character character)
        {
            _name.text = character.Name;
            _image.sprite = character.Avatar;
        }

        public void SetSpeaking(bool isSpeaking)
        {
            _image.color = isSpeaking
                ? DialogueUtils.SpeakerColor
                : DialogueUtils.ListenerColor;
        }

        public void Show()
        {
            if (_isVisible)
            {
                return;
            }

            _isVisible = true;
            _effect.DoEffect();
        }

        public void Hide()
        {
            if (!_isVisible)
            {
                return;
            }

            _isVisible = false;
            _effect.ReverseEffect();
        }
    }
}
