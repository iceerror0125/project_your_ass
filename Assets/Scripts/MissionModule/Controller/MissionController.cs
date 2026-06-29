using System;
using MissionModule.Model;
using MissionModule.Repository;
using MissionModule.View;
using UnityEngine;

namespace MissionModule.Controller
{
    public class MissionController : MonoBehaviour
    {
        [SerializeField] private MissionRepository _repository;
        [SerializeField] private MissionUI _ui;
        
        private MissionData _currentMission;

        private void OnEnable()
        {
            ObserverSystem.Subscribe<ActiveMissionMessage>(OnMessageReceived);
            ObserverSystem.Subscribe<DialogueStartedMessage>(OnDialogueStarted);
            ObserverSystem.Subscribe<DialogueEndedMessage>(OnDialogueEnded);
        }

        private void OnDisable()
        {
            ObserverSystem.UnSubscribe<ActiveMissionMessage>(OnMessageReceived);
            ObserverSystem.UnSubscribe<DialogueStartedMessage>(OnDialogueStarted);
            ObserverSystem.UnSubscribe<DialogueEndedMessage>(OnDialogueEnded);
        }

        private void OnDialogueStarted(DialogueStartedMessage _)
        {
            _ui.MoveHandlerForDialogue();
        }

        private void OnDialogueEnded(DialogueEndedMessage _)
        {
            _ui.RestoreHandlerPosition();
        }

        private void OnMessageReceived(ActiveMissionMessage _)
        {
            if (IsEmptyMission())
            {
                SetMission();
                RegisterMissionEvent();
                SetUI();
                
                Debug.Log("Start Mission: " + _currentMission.name);
            }
        }

        #region UI

        private void SetUI()
        {
            _ui.SetText(_currentMission.name);
        }

        #endregion

        #region Mission
        private bool IsEmptyMission()
        {
            return _currentMission.id == 0;
        }

        private void SetMission()
        {
            _currentMission = _repository.GetCurrentMission();
            _currentMission.ChangeState(MissionState.InProgress);
        }

        private void MarkAsCompleted()
        {
            _repository.UpdateMissionState(_currentMission.id, MissionState.Completed);
        }
        
        private void ResetMission()
        {
            _currentMission = new MissionData();
        }
        
        #endregion

        #region Mission Events
        private void CheckMissionProgress(MissionObjective objective)
        {
            if (_currentMission.objective != objective)
                return;
            
            _currentMission.PlusOne();
            
            if (_currentMission.IsCompleted())
            {
                //todo: reward??
                //todo: update UI
                
                Debug.Log("Mission Completed: " + _currentMission.name);
                MarkAsCompleted();
                UnregisterMissionEvent();
                ResetMission();
            }
           
            //todo: update UI
        }

        private void RegisterMissionEvent()
        {
            MissionTriggerEvents.Raised += CheckMissionProgress;
        }

        private void UnregisterMissionEvent()
        {
            MissionTriggerEvents.Raised -= CheckMissionProgress;
        }
        #endregion
    }
}
