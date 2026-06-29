using System;
using UnityEngine;

namespace MissionModule.Model
{
    [Serializable]
    public struct MissionData
    {
        public int id;
        public string name;
        public string description;
        public int progress;
        public int total;
        public MissionObjective objective;
        public MissionState state;

        public void PlusOne()
        {
            progress += 1;
        }

        public bool IsCompleted()
        {
            return progress >= total;
        }
        
        public void ChangeState(MissionState newState)
        {
            state = newState;
        }
    }
}