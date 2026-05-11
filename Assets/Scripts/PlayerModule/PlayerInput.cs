using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerModule
{
    public class PlayerInput  : MonoBehaviour
    {
        [SerializeField] private InputActionAsset _inputActionAsset;
        
        private InputAction _movement;
        private InputAction _jump;
        
        public InputAction Movement => _movement;
        public InputAction Jump => _jump;

        private void Awake()
        {
            InitInputAction();
        }

        private void InitInputAction()
        {
            _movement = _inputActionAsset.FindAction("Move");
            _jump = _inputActionAsset.FindAction("Jump");
        }
    }
}