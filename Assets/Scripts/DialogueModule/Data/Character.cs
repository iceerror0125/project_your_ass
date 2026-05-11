using UnityEngine;

namespace DialogueModule.Data
{
    public struct Character
    {
        public readonly Sprite avatar;
        public readonly string name;
        public readonly Emotion emotion;

        public Character(string name, Emotion emotion, Sprite avatar)
        {
            this.name = name;
            this.emotion = emotion;
            this.avatar = avatar;
        }
    }
}