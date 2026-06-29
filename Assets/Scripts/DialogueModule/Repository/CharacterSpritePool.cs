using System;
using AYellowpaper.SerializedCollections;
using DialogueModule.Model;
using UnityEngine;
using UnityEngine.Serialization;

namespace DialogueModule.Repository
{
    [CreateAssetMenu(
        fileName = "CharacterSpritePool",
        menuName = "Dialogue/Character Sprite Pool")]
    public sealed class CharacterSpritePool : ScriptableObject
    {
        [FormerlySerializedAs("defaultSprite")]
        [SerializeField] private Sprite _defaultSprite;

        [FormerlySerializedAs("spriteData")]
        [SerializedDictionary("Character Name", "Emotion Sprites")]
        [SerializeField] private SerializedDictionary<string, EmotionSpriteDictionary> _spriteData;

        public Sprite GetSprite(string characterName, Emotion emotion)
        {
            if (string.IsNullOrWhiteSpace(characterName) || _spriteData == null)
            {
                return _defaultSprite;
            }

            if (!_spriteData.TryGetValue(characterName, out EmotionSpriteDictionary emotionSprites) ||
                emotionSprites == null ||
                !emotionSprites.TryGetValue(emotion, out Sprite sprite) ||
                sprite == null)
            {
                return _defaultSprite;
            }

            return sprite;
        }
    }

    [Serializable]
    public sealed class EmotionSpriteDictionary : SerializedDictionary<Emotion, Sprite>
    {
    }
}
