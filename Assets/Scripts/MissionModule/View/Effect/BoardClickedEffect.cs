using TriInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MissionModule.View.Effect
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(HingeJoint))]
    public class BoardClickedEffect : MonoBehaviour, IPointerClickHandler
    {
        private Rigidbody _rb;
        private HingeJoint _hj;
        public float force;

        private const float CLICK_DURATION_TIME = 10f;
        private float _lastClickTime;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _hj = GetComponent<HingeJoint>();
        }

        [Button("Add Force")]
        private void AddForce()
        {
            if (_rb == null || _hj == null)
                return;
            
            _rb.AddForce(Vector3.forward * force, ForceMode.Impulse);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!CanClick())
                return;
            
            AddForce();
            
            _lastClickTime = Time.time;
        }

        private bool CanClick()
        {
            return Time.time - _lastClickTime > CLICK_DURATION_TIME;
        }
    }
}