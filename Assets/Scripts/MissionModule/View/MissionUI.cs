using EffectModule;
using LitMotion;
using TMPro;
using TriInspector;
using UnityEngine;

namespace MissionModule.View
{
    public sealed class MissionUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _missionDescription;

        [Header("Dialogue Movement")]
        [SerializeField] private Transform _handler;
        [SerializeField] private float _dialogueY = 8f;
        [SerializeField, Min(0f)] private float _moveDuration = 0.5f;

        private TypeWriterEffect _textEffect;
        private LocalYMotion _handlerMotion;

        private void Awake()
        {
            _textEffect = new TypeWriterEffect(_missionDescription);

            if (_handler == null)
            {
                Debug.LogError("MissionUI requires a handler transform.", this);
                return;
            }

            _handlerMotion = new LocalYMotion(_handler);
        }

        private void OnDestroy()
        {
            _handlerMotion?.Cancel();
        }

        public void SetText(string text)
        {
            _textEffect.SetText(text ?? string.Empty).DoEffect();
        }

        public void MoveHandlerForDialogue()
        {
            _handlerMotion?.MoveTo(_dialogueY, _moveDuration);
        }

        public void RestoreHandlerPosition()
        {
            _handlerMotion?.Restore(_moveDuration);
        }

        [Button("Test")]
        private void TestTextTransition()
        {
            string currentText = _missionDescription.text;
            const string NewText = "Now I'm a very strong man";

            _textEffect
                .SetText(currentText)
                .SetDuration(0.5f)
                .SetCallback(TypeNewText)
                .DoInverseEffect();

            void TypeNewText()
            {
                _textEffect.SetText(NewText).DoEffect();
            }
        }
    }

    internal sealed class LocalYMotion
    {
        private readonly Transform _target;
        private readonly float _initialY;
        private MotionHandle _motion;

        public LocalYMotion(Transform target)
        {
            _target = target;
            _initialY = target.localPosition.y;
        }

        public void MoveTo(float targetY, float duration)
        {
            _motion.TryCancel();

            float currentY = _target.localPosition.y;
            if (duration <= 0f || Mathf.Approximately(currentY, targetY))
            {
                SetLocalY(targetY);
                return;
            }

            _motion = LMotion.Create(currentY, targetY, duration)
                .Bind(SetLocalY);
        }

        public void Restore(float duration)
        {
            MoveTo(_initialY, duration);
        }

        public void Cancel()
        {
            _motion.TryCancel();
        }

        private void SetLocalY(float y)
        {
            Vector3 position = _target.localPosition;
            position.y = y;
            _target.localPosition = position;
        }
    }
}
