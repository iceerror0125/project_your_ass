using System;
using LitMotion;
using TMPro;

namespace DialogueModule.Effect
{
    public class TypeWriterEffect
    {
        private readonly float _duration = 1; // in second
        private readonly TextMeshProUGUI _text;
        
        private MotionHandle _handle;
        
        public Action onEffectCompleted;
        
        public TypeWriterEffect(TextMeshProUGUI textGUI)
        {
            _text = textGUI;
        }

        public void DoEffect(string text)
        {
            SetText(text);
            SetVisibleCharacters(0);
            
            _handle = LMotion.Create(0, text.Length, _duration).WithOnComplete(OnEffectCompleted).Bind(SetVisibleCharacters);
        }

        public void SetTextInstantly(string text)
        {
            CancelEffect();
            SetVisibleCharacters(text.Length);
            SetText(text);
        }

        private void SetVisibleCharacters(int count)
        {
            _text.maxVisibleCharacters = count;
        }

        private void SetText(string text)
        {
            _text.text = text;
        }

        private void CancelEffect()
        {
            _handle.Cancel();
        }

        private void OnEffectCompleted()
        {
            onEffectCompleted?.Invoke();
        }
    }
}
