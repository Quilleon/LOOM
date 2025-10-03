using System;
using System.Collections;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class EnemyBehaviour : MonoBehaviour
{
    private Animator _anim;
    private Rigidbody _rb;

    [SerializeField] private float walkSpeed = 3;
    private bool activated, stopVeloctiy;
    private bool dead, plannedAttack;
    
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
            if (!stopVeloctiy)
            {
                //print("Walking towards player");
                _rb.linearVelocity = transform.forward.normalized * walkSpeed;
            }

            if (PlayerInReach() && !plannedAttack)
            {
                plannedAttack = true;
                var randomDelay = UnityEngine.Random.Range(2, 4);
                StartCoroutine(AttackDelay(randomDelay));
            }
        }
        
        // Animations
        if (dead)
        {
            _anim.Play("Death");
        }
        else if (!activated)
        {
            print("Not activated");
            _anim.Play("Idle");
        }
        else if (stopVeloctiy)
        {
            //print("Attacking");
            _rb.linearVelocity = Vector3.zero;
            //_anim.Play("Attack");
        }
        else if (_rb.linearVelocity.magnitude > walkSpeed * 0.8f)
        {
            _anim.Play("Walk");
        }
        else // standing still for some reason
        {
            //print(_rb.linearVelocity.magnitude);
            //Debug.Log("Standing still for some reason");
            _anim.Play("Idle");
        }
    }

    private IEnumerator AttackDelay(float delay)
    {
        
        yield return new WaitForSeconds(delay);
        if (PlayerInReach())
        {
            StartCoroutine(Attack());
        }
        else
        {
            plannedAttack = false;
        }
    }

    private IEnumerator Attack()
    {
        stopVeloctiy = true;
        
        _anim.Play("AttackWindup");
        
        var attackWindUp = .8f;
        yield return new WaitForSeconds(attackWindUp);
        
        // Spawn attack box
        _anim.Play("Attack");

        var attackRecovery = 1f;
        yield return new WaitForSeconds(attackRecovery);
        
        plannedAttack = false;
        stopVeloctiy = false;
    }

    private void Behaviour()
    {
        
    }

    private bool PlayerInReach()
    {
        return true;
    }
    
    private bool CanSeePlayer()
    {
        
        
        return true;
    }
}
