using System;
using TriInspector;
using UnityEngine;

namespace MissionModule.Testing
{
    public class MissionTesting : MonoBehaviour
    {
        [Button]
        public void StartMission()
        {
            ObserverSystem.Announce(new ActiveMissionMessage());
        }
    }
}
