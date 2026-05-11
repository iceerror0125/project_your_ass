using System;
using System.Collections.Generic;
using DialogueModule.AssetData;
using DialogueModule.Data;
using Ink.Runtime;
using UnityEngine;

namespace DialogueModule.Ink
{
    public class InkDataProcessor : MonoBehaviour, IDialogue
    {
        [SerializeField] private TextAsset inkJSON;
        [SerializeField] private CharacterSpritePool characterSpritePool;
        private InkReader _inkReader;

        private void Awake()
        {
            _inkReader = new InkReader(inkJSON.text);
        }

         public DialogueData GetDialogueData()
        {   
            DialogueData data = DialogueData.Empty;
            bool canContinue = _inkReader.ToNextLine(out string text);

            if (!canContinue) 
                return data;
            
            data.speaker = GetSpeaker();
            data.message = text;
            data.choices = _inkReader.GetChoices();
            data.side = GetCharacterSide();

            return data;
        }

        public bool ChooseChoice(Choice choice)
        {
            int choiceCount = _inkReader.GetChoices().Count;
            if (choiceCount <= choice.index) return false;
            _inkReader.ChooseChoice(choice.index);
            return _inkReader.CanContinue();
        }

        private Character GetSpeaker()
        {
            List<string> tags = _inkReader.GetCurrentTags();

            Character nullCharacter = new Character("Null Character", Emotion.None, null);
            if (tags.Count == 0)
            {
                return nullCharacter;
            }
            
            foreach (var tag in tags)
            {
                var split = tag.Split(':');
                string charName =  split[0];
                Emotion emotion = GetEmotionMapping(split[1]);
                Sprite sprite = GetSprite(charName, emotion);
                
                Character character = new Character(charName, emotion, sprite);
                return character;
            }

            return nullCharacter;
        }

        private Emotion GetEmotionMapping(string emotion)
        {
            string lower = emotion.ToLower();
            switch (lower)
            {
                case "happy" : return Emotion.Happy;
                case "surprise" : return Emotion.Surprise;
                case "annoy" : return Emotion.Annoy;
                default: return Emotion.None;
            }
        }

        private CharacterSide GetCharacterSide()
        {
            string side = _inkReader.GetVariable("side");
            return side == "left" ? CharacterSide.Left : CharacterSide.Right;
        }
        
        private Sprite GetSprite(string name, Emotion emotion)
        {
            return characterSpritePool.GetSprite(name, emotion);
        }
    }
}