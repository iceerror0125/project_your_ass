using System.Collections.Generic;
using MissionModule.Model;
using UnityEngine;

namespace MissionModule.Repository
{
    [CreateAssetMenu(menuName = "Missions/MissionRepository")]
    public class MissionRepository : ScriptableObject
    {
        [SerializeField] private List<MissionData> _missions =  new List<MissionData>();
        
        public MissionData GetCurrentMission()
        {
            foreach (var mission in _missions)
            {
                if (mission.state == MissionState.NotStarted)
                    return mission;
            }
            return new MissionData();
        }
        
        public void UpdateMissionState(int missionId, MissionState newState)
        {
            for (int i = 0; i < _missions.Count; i++)
            {
                if (_missions[i].id != missionId)
                    continue;

                MissionData mission = _missions[i];
                mission.ChangeState(newState);
                _missions[i] = mission;
                return;
            }
        }
    }
}