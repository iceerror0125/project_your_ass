using System;
using System.Collections.Generic;

namespace DialogueModule.Model
{
    public enum CharacterSide
    {
        Left,
        Right
    }

    public readonly struct DialogueChoice
    {
        public DialogueChoice(int index, string text)
        {
            Index = index;
            Text = text ?? string.Empty;
        }

        public int Index { get; }
        public string Text { get; }
    }

    public sealed class DialogueData
    {
        private static readonly IReadOnlyList<DialogueChoice> NoChoices = Array.Empty<DialogueChoice>();

        private DialogueData()
        {
            IsComplete = true;
            Speaker = Character.Empty;
            Side = CharacterSide.Left;
            Message = string.Empty;
            Choices = NoChoices;
        }

        public DialogueData(
            Character speaker,
            CharacterSide side,
            string message,
            IReadOnlyList<DialogueChoice> choices)
        {
            Speaker = speaker;
            Side = side;
            Message = message ?? string.Empty;
            Choices = choices ?? NoChoices;
        }

        public static DialogueData Complete { get; } = new DialogueData();

        public Character Speaker { get; }
        public CharacterSide Side { get; }
        public string Message { get; }
        public IReadOnlyList<DialogueChoice> Choices { get; }
        public bool IsComplete { get; }
    }

    public interface IDialogue
    {
        void Restart();
        DialogueData Advance();
        bool TryChoose(DialogueChoice choice);
    }
}
