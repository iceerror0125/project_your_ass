using System.Xml;
using DialogueModule.Data;

namespace DialogueModule.UI
{
    // show and hide character avatar
    public class UICharacterRegister
    {
        private readonly UICharacter _leftCharacter;
        private readonly UICharacter _rightCharacter;
        
        private bool _leftSide;
        private bool _rightSide;

        private UICharacter selectedCharacter;
        public UICharacterRegister(UICharacter leftCharacter, UICharacter rightCharacter)
        {
            _leftCharacter = leftCharacter;
            _rightCharacter = rightCharacter;
        }

        public void SetSpeakerAndSide(Character data, CharacterSide side)
        {
            ToListener();
            RegisterSide(side);
            SetCharacterData(selectedCharacter, data);
            selectedCharacter.Speak();
        }

        private void ToListener()
        {
            if (selectedCharacter == null)
                return;
            
            selectedCharacter.Listen();
        }

        private void RegisterSide(CharacterSide side)
        {
            if (side == CharacterSide.Left)
            {
                SetLeftSide(true);
                _leftCharacter.EnterConversation();
                selectedCharacter = _leftCharacter;
            }
            else
            {
                SetRightSide(true);
                _rightCharacter.EnterConversation();
                selectedCharacter =  _rightCharacter;
            }
        }

        private void SetCharacterData(UICharacter ui, Character data)
        {
            ui.SetAvatar(data.avatar);
            ui.SetName(data.name);
        }

        public bool HasAlreadyRegistered(CharacterSide side)
        {
            switch (side)
            {
                case CharacterSide.Left when _leftSide:
                case CharacterSide.Right when _rightSide:
                    return true;
                default:
                    return false;
            }
        }

        public void WithdrawCharacter(CharacterSide side)
        {
            switch (side)
            {
                case CharacterSide.Left:
                    SetLeftSide(false);
                    _rightCharacter.ExitConversation();
                    break;
                case CharacterSide.Right:
                    SetRightSide(false);
                    _leftCharacter.ExitConversation();
                    break;
            }
        }

        private void SetLeftSide(bool enabled)
        {
            _leftSide = enabled;
        }

        private void SetRightSide(bool enabled)
        {
            _rightSide = enabled;
        }
    }
}