using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(Rigidbody))]
public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject enemyDeath;
    [SerializeField] private GameObject enemyAttack;
    [SerializeField] private Transform enemyAttackSpawn;
    
    private Transform _player;
    
    private Animator _anim;
    public SpriteRenderer spriteRenderer;
    private Rigidbody _rb;
    private NavMeshAgent _agent;
    private BillboardEffect _billboardEffect;

    [SerializeField] private float maxHealth = 100, currentHealth;
    
    
    [SerializeField] private float walkSpeed = 3;
    [SerializeField] private float reach = 5, sightRange = 10;
    private bool activated, walking;
    public bool isDead, isFrozen;
    private bool _wasFrozen;
    private bool deathAnimPlaying, plannedAttack;
    
    void Start()
    {
        if (GameObject.Find("Player"))
        {
            _player = GameObject.Find("Player").transform;
        }
        else
        {
            Debug.LogError("Player not found!");
            Instantiate(enemyDeath, transform.position, transform.rotation);
            Destroy(gameObject);
            return;
        }
        
        
        _rb = GetComponent<Rigidbody>();
        
        _anim = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        _agent = GetComponent<NavMeshAgent>();

        _billboardEffect = GetComponentInChildren<BillboardEffect>();
        
        _agent.speed = 0;
        _agent.acceleration = 100; // Speed should be instant
        _agent.stoppingDistance = reach * 3/5; // Should stop when player is in reach
        StartCoroutine(SetNewPosition());
        //_agent.enabled = false; // Enables when walking
        
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        CanSeePlayer();
        
        if (isFrozen)
        {
            print("Enemy Frozen");
            _anim.speed = 0;
            activated = false;
            StopAllCoroutines();
        }
        else if (_wasFrozen != isFrozen && _wasFrozen)
        {
            print("Unfrozen");
            _anim.speed = 1;
        }

        _wasFrozen = isFrozen;
        
        currentHealth = math.clamp(currentHealth, 0, maxHealth);
        if (!isDead && currentHealth <= 0)
        {
            isDead = true;
        }
    }

    private void FixedUpdate()
    {
        if (!isDead && !activated && CanSeePlayer() && !isFrozen)
        {
            print("Activated");
            Activate();
        }

        if (activated)
        {
            _agent.speed = walking ? walkSpeed : 0;
            
            if (walking)
            {
                print("Walking towards player");
                //_rb.linearVelocity = transform.forward.normalized * walkSpeed;
                //_agent.SetDestination(_player.position);
            }

            if (PlayerInReach() && !plannedAttack)
            {
                plannedAttack = true;
                var randomDelay = UnityEngine.Random.Range(3, 5);
                StartCoroutine(AttackDelay(randomDelay));
            }
        }
        else
        {
            _agent.speed = 0;
        }
        
        // Animations
        if (isDead && !deathAnimPlaying)
        {
            if (enemyDeath)
            {
                var death = Instantiate(enemyDeath, transform.position, quaternion.identity);
                death.GetComponentInChildren<Animator>().Play("Death");
                Destroy(gameObject);
            }
            
            /*
            // Stops ongoing attacks
            StopAllCoroutines();
            
            deathAnimPlaying = true;
            activated = false;
            walking = false;
            
            _anim.Play("Death");
            */
        }
        else if (!isDead && !activated)
        {
            print("Not activated");
            _anim.Play("Idle");
        }
        else if (!walking)
        {
            //print("Attacking");
            //_rb.linearVelocity = Vector3.zero;
            _agent.speed = 0;
            //_anim.Play("Attack");
        }
        else if (walking)//(_rb.linearVelocity.magnitude > walkSpeed * 0.8f)
        {
            _anim.Play("Walk");
        }
        else // standing still for some reason
        {
            //print(_rb.linearVelocity.magnitude);
            //Debug.Log("Standing still for some reason");
            //_anim.Play("Idle");
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
        walking = false;
        
        _anim.Play("AttackWindup");

        
        var attackWindUp = .8f;
        
        
        yield return new WaitForSeconds(attackWindUp/2);
        
        _billboardEffect.enabled = false;
        
        yield return new WaitForSeconds(attackWindUp/2);
        
        // Spawn attack box
        if (enemyAttack)
        {
            var attack = Instantiate(enemyAttack, enemyAttackSpawn ? enemyAttackSpawn.position : transform.position, transform.rotation);
            attack.tag = "EnemyAttack";
        }
        
        _anim.Play("Attack");

        var attackRecovery = 1f;
        yield return new WaitForSeconds(attackRecovery);
        
        _billboardEffect.enabled = true;
        
        plannedAttack = false;
        walking = true;
    }

    private IEnumerator SetNewPosition()
    {
        _agent.SetDestination(_player.position);

        yield return new WaitForSeconds(1f);
        
        StartCoroutine(SetNewPosition());
    }

    private void Activate()
    {
        activated = true;
        walking = true;
        _agent.speed = walkSpeed;
    }
    

    private bool PlayerInReach()
    {
        var playerVector = _player.position - transform.position;
        
        return playerVector.magnitude < reach;
    }
    
    private bool CanSeePlayer()
    {
        var playerVector = (_player.position - transform.position).normalized;
        Debug.DrawRay(transform.position, playerVector * sightRange, Color.yellow);
        Physics.Raycast(transform.position, playerVector, out RaycastHit hit, sightRange);
        
        if (hit.collider != null)
            return hit.collider.gameObject.layer == 7; // Player layer
        return false;
    }


    public void TakeDamage(float damage)
    {
        if (isDead) return;

        if (!activated)
        {
            Activate(); // If the player manages to shoot them (through walls for example)
        }
        
        currentHealth -= damage;
    }

    public void ChangeMaterial(Material mat)
    {
        spriteRenderer.material = mat;
    }
}
