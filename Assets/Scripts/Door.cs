using System;
using UnityEngine;

public class Door : MonoBehaviour
{
    void Start()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 7) // Player Layer
        {
            GetComponentInParent<EnemyManager>().ReloadLevel();
        }
    }
}
