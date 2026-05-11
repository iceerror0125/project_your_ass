using System;
using DialogueModule.Effect;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueModule.UI
{
    public class UICharacter : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private ImageFadeAndMovementEffect _effect;

        public bool _hasEntered;
        
        public Action onEnterConversation;
        
        public void SetName(string name)
        {
            _name.text = name;
        }

        public void SetAvatar(Sprite sprite)
        {
            _image.sprite = sprite;
            SetImageColor(DialogueUtils.ListenerColor);
        }

        public void Speak()
        {
            SetImageColor(DialogueUtils.SpeakerColor);
        }

        public void Listen()
        {
            SetImageColor(DialogueUtils.ListenerColor);
        }

        private void SetImageColor(Color color)
        {
            _image.color = color;
        }
        
        public void EnterConversation()
        {
            if (_hasEntered)
            {
               return;
            }
            
            _hasEntered = true;
            _effect.DoEffect();
        }
        public void ExitConversation()
        {
            if (!_hasEntered)
            {
                return;
            }
            
            _hasEntered = false;
            _effect.ReverseEffect();
        }
    }
}