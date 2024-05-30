using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnContact : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Destroy the object that enters the trigger
        Destroy(other.gameObject);
    }

}
