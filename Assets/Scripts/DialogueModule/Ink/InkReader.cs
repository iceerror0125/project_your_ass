using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;

namespace DialogueModule.Ink
{
    public class InkReader 
    {
        private readonly Story _story;

        public InkReader(string json)
        {
            _story = new Story(json);
        }

        public int GetCurrentChoices()
        {
            return _story.currentChoices.Count;
        }

        public void ChooseChoice(int index)
        {
            _story.ChooseChoiceIndex(index);
        }

        public bool CanContinue()
        {
            return _story.canContinue;
        }

        public string Continue()
        {
            return _story.Continue();
        }
        
        public bool ToNextLine(out string text)
        {
            if (_story.canContinue) {
                text = _story.Continue();
                return true;
            }
            
            text = "";
            return false;
        }
        
        public List<Choice> GetChoices()
        {
            if (_story.currentChoices.Count > 0) {
               return _story.currentChoices;
            }
            return new List<Choice>();
        }

        public List<string> GetCurrentTags()
        {
            return _story.currentTags;
        }

        public string GetVariable(string variableName)
        {
            var value = _story.variablesState[variableName];
            return value != null ? value.ToString() : string.Empty;
        }
    }
}
