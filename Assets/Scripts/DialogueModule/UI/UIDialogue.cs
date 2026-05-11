using System.Collections.Generic;
using DialogueModule.AssetData;
using DialogueModule.Data;
using DialogueModule.Ink;
using Ink.Runtime;
using TriInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DialogueModule.UI
{
    public class UIDialogue : MonoBehaviour, IPointerDownHandler
    {
        [Header("Ref")]
        [SerializeField] private InkDataProcessor test;
        
        [Header("Character")]
        [SerializeField] private UICharacter _leftCharacter;
        [SerializeField] private UICharacter _rightCharacter;
        
        [Header("Components")]
        [SerializeField] private List<UIOption> _options;
        [SerializeField] private UITextBox _textbox;
        private UICharacterRegister _sideRegister;
        
        private IDialogue dialogue;

        private void Awake()
        {
            dialogue = test;
            _sideRegister = new UICharacterRegister(_leftCharacter, _rightCharacter);
        }

        [Button("Start dialogue")]
        public void StartDialogue()
        {
            DialogueData data = dialogue.GetDialogueData();
            if (data.IsEmpty)
                return;
            
            LoadUI(data);
        }
        
        private void LoadUI(DialogueData data)
        {
            _sideRegister.SetSpeakerAndSide(data.speaker, data.side);
            _textbox.SetMessage(data.message);
            
            SetUpOption(data.choices);
        }

        private void SetUpOption(List<Choice> choices)
        {
            ResetOption();
            SetOption(choices);
        }
        
        private void ResetOption()
        {
            foreach (var option in _options)
            {
                if (option.IsEnabled()) 
                    option.Disable();
            }
        }

        private void SetOption(List<Choice> choices)
        {
            for (int i = 0; i < choices.Count; i++)
            {
                UIOption option = _options[i];
                option.Enable();
                option.SetText(choices[i].text);
                option.SetChoice(choices[i]);
                option.onClicked += OnOptionClicked;
            }
        }

        private void OnOptionClicked(Choice choice)
        {
            bool canContinue = dialogue.ChooseChoice(choice);
            if (!canContinue)
            { 
                Debug.LogWarning("Can't continue conversation");
                return;
            }
            
            StartDialogue();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            StartDialogue();
        }
    }
}