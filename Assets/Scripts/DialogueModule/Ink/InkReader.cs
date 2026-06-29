using System;
using System.Collections.Generic;
using Ink.Runtime;

namespace DialogueModule.Ink
{
    public sealed class InkReader
    {
        private readonly Story _story;

        public InkReader(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new ArgumentException("Ink JSON cannot be empty.", nameof(json));
            }

            _story = new Story(json);
        }

        public bool CanContinue => _story.canContinue;
        public string CurrentText { get; private set; } = string.Empty;
        public IReadOnlyList<Choice> CurrentChoices => _story.currentChoices;
        public IReadOnlyList<string> CurrentTags => _story.currentTags;

        public string Continue()
        {
            if (!CanContinue)
            {
                return CurrentText;
            }

            CurrentText = _story.Continue() ?? string.Empty;
            return CurrentText;
        }

        public bool TryChoose(int index)
        {
            if (index < 0 || index >= _story.currentChoices.Count)
            {
                return false;
            }

            _story.ChooseChoiceIndex(index);
            return true;
        }

        public bool TryGetVariable(string variableName, out string value)
        {
            value = string.Empty;
            if (string.IsNullOrWhiteSpace(variableName) ||
                !_story.variablesState.GlobalVariableExistsWithName(variableName))
            {
                return false;
            }

            object variable = _story.variablesState[variableName];
            if (variable == null)
            {
                return false;
            }

            value = variable.ToString();
            return true;
        }
    }
}
