using System;
using UnityEngine;

public class TriggerCulling : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            Debug.Log($"GameObject: {gameObject.name} is on");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            Debug.Log($"GameObject: {gameObject.name} is off");
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("MainCamera"))
        {
            Debug.Log($"GameObject: {gameObject.name} is on / Collision");
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("MainCamera"))
        {
            Debug.Log($"GameObject: {gameObject.name} is off / Collision");
        }
    }
}
