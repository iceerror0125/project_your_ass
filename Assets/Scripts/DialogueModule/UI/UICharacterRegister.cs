using DialogueModule.Model;

namespace DialogueModule.UI
{
    public sealed class UICharacterRegister
    {
        private readonly UICharacter _leftCharacter;
        private readonly UICharacter _rightCharacter;
        private UICharacter _activeSpeaker;

        public UICharacterRegister(UICharacter leftCharacter, UICharacter rightCharacter)
        {
            _leftCharacter = leftCharacter;
            _rightCharacter = rightCharacter;
        }

        public void Present(Character character, CharacterSide side)
        {
            UICharacter nextSpeaker = side == CharacterSide.Left
                ? _leftCharacter
                : _rightCharacter;

            if (_activeSpeaker != null && _activeSpeaker != nextSpeaker)
            {
                _activeSpeaker.SetSpeaking(false);
            }

            nextSpeaker.SetCharacter(character);
            nextSpeaker.Show();
            nextSpeaker.SetSpeaking(true);
            _activeSpeaker = nextSpeaker;
        }

        public void Clear()
        {
            _leftCharacter.Hide();
            _rightCharacter.Hide();
            _activeSpeaker = null;
        }
    }
}
