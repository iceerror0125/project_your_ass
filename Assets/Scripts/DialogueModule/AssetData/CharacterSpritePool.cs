using System;
using AYellowpaper.SerializedCollections;
using DialogueModule.Data;
using UnityEngine;

namespace DialogueModule.AssetData
{
    [CreateAssetMenu(fileName = "CharacterSpritePool", menuName = "CharacterSpritePool")]
    public class CharacterSpritePool : ScriptableObject
    {
        [SerializeField] private Sprite defaultSprite;

        [SerializedDictionary("Character Name", "Emotions")]
        [SerializeField] private SerializedDictionary<string, EmotionSpriteDictionary> spriteData;

        public Sprite GetSprite(string name,  Emotion emotion)
        {
            if (spriteData != null && spriteData.TryGetValue(name, out var emotions))
            {
                if (emotions != null && emotions.TryGetValue(emotion, out var sprite))
                {
                    return sprite != null ? sprite : defaultSprite;
                }
            }

            return defaultSprite;
        }
    }

    [Serializable]
    public class EmotionSpriteDictionary : SerializedDictionary<Emotion, Sprite> { }
}