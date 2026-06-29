using TriInspector;
using UnityEngine;

namespace Test
{
    public class TestAddForece : MonoBehaviour
    {
        private Rigidbody _rb;
        public float force;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        [Button("Add Forece")]
        private void AddForce()
        {
            _rb.AddForce(Vector3.forward * force, ForceMode.Impulse);
        }
    }
}
