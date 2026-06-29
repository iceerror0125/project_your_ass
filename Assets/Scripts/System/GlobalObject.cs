using UnityEngine;

namespace System
{
    public class GlobalObject: MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}