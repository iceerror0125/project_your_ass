using System;
using MissionModule;
using MissionModule.Model;
using UnityEngine;

namespace Test
{
    public class TestItem : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;
            MissionTriggerEvents.Raised.Invoke(MissionObjective.CollectB);
        }
    }
}