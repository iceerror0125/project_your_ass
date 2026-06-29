using System;
using System.Collections.Generic;
using DialogueModule.Model;
using DialogueModule.Repository;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.Serialization;

namespace DialogueModule.Ink
{
    public static class InkSpeakerTagParser
    {
        private const char Separator = ':';

        public static bool TryParse(string tag, out string characterName, out Emotion emotion)
        {
            characterName = string.Empty;
            emotion = Emotion.None;

            if (string.IsNullOrWhiteSpace(tag))
            {
                return false;
            }

            int separatorIndex = tag.IndexOf(Separator);
            if (separatorIndex <= 0 || separatorIndex >= tag.Length - 1)
            {
                return false;
            }

            characterName = tag.Substring(0, separatorIndex).Trim();
            string emotionName = tag.Substring(separatorIndex + 1).Trim();
            if (characterName.Length == 0 || !TryParseEmotion(emotionName, out emotion))
            {
                characterName = string.Empty;
                return false;
            }

            return true;
        }

        private static bool TryParseEmotion(string value, out Emotion emotion)
        {
            if (string.Equals(value, "annoyed", StringComparison.OrdinalIgnoreCase))
            {
                emotion = Emotion.Annoy;
                return true;
            }

            return Enum.TryParse(value, true, out emotion) && emotion != Emotion.None;
        }
    }

    public sealed class InkDataProcessor : MonoBehaviour, IDialogue
    {
        private const string CharacterSideVariable = "side";

        [FormerlySerializedAs("inkJSON")]
        [SerializeField] private TextAsset _inkJson;

        [FormerlySerializedAs("characterSpritePool")]
        [SerializeField] private CharacterSpritePool _characterSpritePool;

        private InkReader _reader;

        private void Awake()
        {
            TryCreateReader();
        }

        public void Restart()
        {
            _reader = null;
            TryCreateReader();
        }

        public DialogueData Advance()
        {
            if (!TryCreateReader() || !HasContent())
            {
                return DialogueData.Complete;
            }

            string message = _reader.CanContinue ? _reader.Continue() : _reader.CurrentText;
            return new DialogueData(
                ResolveSpeaker(),
                ResolveCharacterSide(),
                message,
                CreateChoices(_reader.CurrentChoices));
        }

        public bool TryChoose(DialogueChoice choice)
        {
            return TryCreateReader() && _reader.TryChoose(choice.Index);
        }

        private bool HasContent()
        {
            return _reader.CanContinue || _reader.CurrentChoices.Count > 0;
        }

        private bool TryCreateReader()
        {
            if (_reader != null)
            {
                return true;
            }

            if (_inkJson == null)
            {
                Debug.LogError("InkDataProcessor requires an Ink JSON asset.", this);
                return false;
            }

            try
            {
                _reader = new InkReader(_inkJson.text);
                return true;
            }
            catch (Exception exception)
            {
                Debug.LogError($"Could not load Ink story: {exception.Message}", this);
                return false;
            }
        }

        private static IReadOnlyList<DialogueChoice> CreateChoices(IReadOnlyList<Choice> inkChoices)
        {
            if (inkChoices.Count == 0)
            {
                return Array.Empty<DialogueChoice>();
            }

            var choices = new DialogueChoice[inkChoices.Count];
            for (int index = 0; index < inkChoices.Count; index++)
            {
                Choice inkChoice = inkChoices[index];
                choices[index] = new DialogueChoice(index, inkChoice.text);
            }

            return choices;
        }

        private Character ResolveSpeaker()
        {
            foreach (string tag in _reader.CurrentTags)
            {
                if (!InkSpeakerTagParser.TryParse(tag, out string name, out Emotion emotion))
                {
                    continue;
                }

                Sprite avatar = _characterSpritePool != null
                    ? _characterSpritePool.GetSprite(name, emotion)
                    : null;
                return new Character(name, emotion, avatar);
            }

            return Character.Empty;
        }

        private CharacterSide ResolveCharacterSide()
        {
            if (_reader.TryGetVariable(CharacterSideVariable, out string value) &&
                Enum.TryParse(value, true, out CharacterSide side))
            {
                return side;
            }

            return CharacterSide.Left;
        }
    }
}
