using UnityEngine;

namespace DialogueModule.Model
{
    public readonly struct Character
    {
        public Character(string name, Emotion emotion, Sprite avatar)
        {
            Name = name ?? string.Empty;
            Emotion = emotion;
            Avatar = avatar;
        }

        public static Character Empty { get; } = new Character(string.Empty, Emotion.None, null);

        public string Name { get; }
        public Emotion Emotion { get; }
        public Sprite Avatar { get; }
    }
}
