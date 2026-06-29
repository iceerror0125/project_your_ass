using System;
using LitMotion;
using TMPro;
using Unity.VisualScripting;

namespace EffectModule
{
    
    public class TypeWriterEffect
    {
        class EffectData
        {
            public string text;
            public float duration = 1;
        }
        
        private readonly TMP_Text _text;
        private EffectData _effectData = new EffectData();
        private MotionHandle _handle;
        private Action _callback;
        
        public TypeWriterEffect(TMP_Text textGUI)
        {
            _text = textGUI;
        }

        public TypeWriterEffect SetText(string text)
        {
            _effectData.text = text;
            return this;
        }
        
        public TypeWriterEffect SetDuration(float duration)
        {
            _effectData.duration = duration;
            return this;
        }

        public TypeWriterEffect SetCallback(Action callback)
        {
            _callback = callback;
            return this;
        }

        public void DoEffect()
        {
            SetUIText(_effectData.text);
            SetVisibleCharacters(0);
            
            _handle = LMotion.Create(0, _effectData.text.Length, _effectData.duration)
                .WithOnComplete(OnEffectCompleted)
                .Bind(SetVisibleCharacters);
        }

        public void DoInverseEffect()
        {
            int textLength = _effectData.text.Length;
            
            SetUIText(_effectData.text);
            SetVisibleCharacters(textLength);
            
            _handle = LMotion.Create(textLength, 0, _effectData.duration)
                .WithOnComplete(OnEffectCompleted)
                .Bind(SetVisibleCharacters);
        }

        private void OnEffectCompleted()
        {
            _effectData = new EffectData();
            _callback?.Invoke();
            _callback = null;
        }

        public void SetTextInstantly(string text)
        {
            CancelEffect();
            SetVisibleCharacters(text.Length);
            SetUIText(text);
        }

        private void SetVisibleCharacters(int count)
        {
            _text.maxVisibleCharacters = count;
        }

        private void SetUIText(string text)
        {
            _text.text = text;
        }

        private void CancelEffect()
        {
            _handle.Cancel();
        }
    }
}
