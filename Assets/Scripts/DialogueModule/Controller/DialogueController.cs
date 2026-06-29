using System;
using DialogueModule.Ink;
using DialogueModule.Model;
using DialogueModule.UI;
using TriInspector;
using UnityEngine;
using UnityEngine.Events;

namespace DialogueModule.Controller
{
    public sealed class DialogueController : MonoBehaviour
    {
        [SerializeField] private UIDialogue _ui;
        [SerializeField] private InkDataProcessor _inkProcessor;

        private IDialogue _dialogue;
        private DialogueState _state = DialogueState.Inactive;

        public event Action DialogueEnded;

        private enum DialogueState
        {
            Inactive,
            ShowingLine,
            AwaitingChoice
        }

        private void Awake()
        {
            _dialogue = _inkProcessor;
        }

        private void OnEnable()
        {
            _ui.BackgroundPressed += HandleBackgroundPressed;
            _ui.OptionSelected += HandleOptionSelected;
        }

        private void OnDisable()
        {
            _ui.BackgroundPressed -= HandleBackgroundPressed;
            _ui.OptionSelected -= HandleOptionSelected;
        }

        [Button("Start dialogue")]
        public void StartDialogue()
        {
            if (_dialogue == null)
            {
                Debug.LogError("DialogueController requires an InkDataProcessor.", this);
                return;
            }

            _dialogue.Restart();
            _state = DialogueState.ShowingLine;
            ObserverSystem.Announce(new DialogueStartedMessage());
            AdvanceDialogue();
        }

        private void HandleBackgroundPressed()
        {
            if (_state == DialogueState.Inactive || _ui.TrySkipTyping())
            {
                return;
            }

            if (_state == DialogueState.ShowingLine)
            {
                AdvanceDialogue();
            }
        }

        private void HandleOptionSelected(DialogueChoice choice)
        {
            if (_state != DialogueState.AwaitingChoice)
            {
                return;
            }

            if (!_dialogue.TryChoose(choice))
            {
                Debug.LogWarning($"Ink rejected dialogue choice index {choice.Index}.", this);
                return;
            }

            AdvanceDialogue();
        }

        private void AdvanceDialogue()
        {
            DialogueData data = _dialogue.Advance();
            if (data.IsComplete)
            {
                EndDialogue();
                return;
            }

            _state = data.Choices.Count > 0
                ? DialogueState.AwaitingChoice
                : DialogueState.ShowingLine;
            _ui.SetVisible(true);
            _ui.Render(data);
        }

        private void EndDialogue()
        {
            if (_state == DialogueState.Inactive)
            {
                return;
            }

            _state = DialogueState.Inactive;
            _ui.SetVisible(false);
            ObserverSystem.Announce(new DialogueEndedMessage());
            DialogueEnded?.Invoke();
        }
    }
}
