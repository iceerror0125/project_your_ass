using TMPro;
using UnityEngine;

namespace EntityModule
{
    public class EntityAnimator
    {
        private readonly Animator _animator;
        
        public EntityAnimator(Animator animator)
        {
            _animator = animator;
        }

        public void Play(string animationName)
        {
            // should check valid name ??
            _animator.Play(animationName);
        }
    }
}