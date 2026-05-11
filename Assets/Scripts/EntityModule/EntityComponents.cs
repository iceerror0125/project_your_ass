using PlayerModule;
using UnityEngine;

namespace EntityModule
{
    public class EntityComponents : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private PlayerInput _playerInput;
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private Camera _mainCamera;
        
        public PlayerInput PlayerInput => _playerInput;
        public CharacterController CharacterController => _characterController;
        public Camera MainCamera => _mainCamera;
        public Animator Animator => _animator;
        
    }
}