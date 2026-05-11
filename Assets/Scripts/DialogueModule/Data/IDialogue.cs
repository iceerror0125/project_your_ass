using System.Collections.Generic;
using Ink.Runtime;

namespace DialogueModule.Data
{
    public struct DialogueData
    {
        public Character speaker;
        public CharacterSide side;
        public string message;
        public List<Choice> choices;

        public static DialogueData Empty => new DialogueData()
        {
            speaker = new Character(),
            side = CharacterSide.Left,
            message = string.Empty,
            choices = new List<Choice>(),
        };
        
        public bool IsEmpty => string.IsNullOrWhiteSpace(message);
    }

    public enum CharacterSide
    {
        Left,
        Right
    }
    
    public interface IDialogue
    {
        public DialogueData GetDialogueData();
        public bool ChooseChoice(Choice choice);
    }
}