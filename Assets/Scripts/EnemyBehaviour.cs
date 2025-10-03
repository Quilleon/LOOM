using System;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class EnemyBehaviour : MonoBehaviour
{
    private Animator _anim;
    private Rigidbody _rb;

    [SerializeField] private float walkSpeed = 3;
    private bool activated;
    
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        
        _anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (!activated && CanSeePlayer())
        {
            print("Activated");
            activated = true;
        }

        if (activated)
        {
            print("Walking towards player");
            _rb.linearVelocity = transform.forward;
        }
    }

    private void Behaviour()
    {
        
    }
    
    private bool CanSeePlayer()
    {
        
        
        return true;
    }
}
