using System;
using System.Collections;
using UnityEngine;

public class DestroyAbility : MonoBehaviour
{
    [SerializeField] private bool destroyOverTime = true;
    [SerializeField] private float destroyTime = 0.5f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (destroyOverTime)
        {
            StartCoroutine(DestroyOverTime());
        }
    }

    private IEnumerator DestroyOverTime()
    {
        yield return new WaitForSeconds(destroyTime);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Destroy when hitting something
    }
}
