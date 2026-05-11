using System;
using DG.Tweening;
using LitMotion;
using TriInspector;
using UnityEngine;
using UnityEngine.UI;
#if DOTWEEN_ENABLED
using DG.Tweening;
#endif

namespace DialogueModule.Effect
{
    [RequireComponent(typeof(CanvasGroup))]
    public class ImageFadeAndMovementEffect : MonoBehaviour
    {
        [SerializeField] private Vector2 _moveFrom;
        [SerializeField] private Vector2 _moveTo;
        [SerializeField] [Range(0, 1)] private float _fadeFrom;
        [SerializeField] [Range(0, 1)] private float _fadeTo;
        [SerializeField] private float _duration;
        
        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;

        public Action onAnimationCompleted;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }
        
      
        [Button("Test")]
        public void DoEffect()
        {
            Move(_moveFrom, _moveTo);
            Fade(_fadeFrom, _fadeTo);
        }

        public void ReverseEffect()
        {
            Move(_moveTo, _moveFrom);
            Fade(_fadeTo, _fadeFrom);
        }

        private void Move(Vector2 from, Vector2 to)
        {
            LMotion.Create(from, to, _duration)
                .WithOnComplete(OnAnimationCompleted)
                .Bind(SetAnchoredPosition);
        }

        private void Fade(float from, float to)
        {
            LMotion.Create(from, to, _duration)
                .Bind(SetAlpha);
        }

        private void SetAnchoredPosition(Vector2 position)
        {
            _rectTransform.anchoredPosition = position;
        }

        private void SetAlpha(float alpha)
        {
            _canvasGroup.alpha = alpha;
        }

        private void OnAnimationCompleted()
        {
            onAnimationCompleted?.Invoke();
        }
        
    }
}
