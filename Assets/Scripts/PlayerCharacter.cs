using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerCharacter : MonoBehaviour
{
    #region Input System
    
    private PlayerInput _input;
    private InputActionAsset _inputAsset;
    
    private Vector2 _movementVector;
    private Vector2 _lookVector;
    
    private bool _leftPunchPressed, _rightPunchPressed;
    private bool _jumpPressed;

    private void Awake()
    {
        _input = GetComponent<PlayerInput>();
        _inputAsset = _input.actions;
    }

    private void OnEnable() { _inputAsset.Enable(); }

    private void OnDisable() { _inputAsset.Disable(); }

    private void UpdateInputs()
    {
        _movementVector = _inputAsset["Move"].ReadValue<Vector2>();
        _lookVector = _inputAsset["Look"].ReadValue<Vector2>();
        
        _leftPunchPressed = _inputAsset["LeftPunch"].WasPressedThisFrame();
        _rightPunchPressed = _inputAsset["RightPunch"].WasPressedThisFrame();
        
        _jumpPressed = _inputAsset["Jump"].WasPressedThisFrame();
    }

    
    #endregion
    
    
    private Rigidbody _rb;
    private Animator _anim;
    
    [SerializeField] float maxHealth = 100f, currentHealth;
    
    [SerializeField] private float _movementSpeed = 5f;
    [SerializeField] private float _jumpForce = 8;
    private float _health;

    private float _inputBuffer;

    [SerializeField] private GameObject _rightArm, _leftArm;
    private Animator _rightAnim, _leftAnim;
    private bool _canRightPunch = true, _canLeftPunch = true;
    [SerializeField] private Upgrade[] _rightUpgrades, _leftUpgrades;

    private Transform _effectSpawn;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _anim = GetComponentInChildren<Animator>();
        
        _camera = transform.GetChild(0);
        _effectSpawn = _camera.GetChild(1);

        // Can cause error
        _rightAnim = _rightArm.GetComponentInParent<Animator>();
        _leftAnim = _leftArm.GetComponentInParent<Animator>();
        
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        Physics.IgnoreLayerCollision(7,8);

        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        if (currentHealth == 0)
        {
            print("Player Is Dead");
            //return;
        }
        
        UpdateInputs();
        UpdateLookDirection(_lookVector, true);

        if (_rightPunchPressed && _canRightPunch)
        {
            //print("Right Hand Punch");
            StartCoroutine(Punch(_rightUpgrades, _rightArm, _rightAnim, true));
        }

        if (_leftPunchPressed && _canLeftPunch)
        {
            //print("Left Hand Punch");
            StartCoroutine(Punch(_leftUpgrades, _leftArm, _leftAnim, false));
        }

        if (_jumpPressed && Grounded())
        {
            print("Jump");
            _rb.linearVelocity += Vector3.up * _jumpForce;
        }
    }

    private float punchLength, punchTime;
    private void LateUpdate()
    {
    }

    private IEnumerator Punch(Upgrade[] upgrades, GameObject arm, Animator armAnim, bool isRight)
    {
        if (isRight) _canRightPunch = false;
        else _canLeftPunch = false;
        
        
        armAnim.Play("Punch");
        
        yield return new WaitForSeconds(.05f);
        
        var armSpawnPoint = arm.transform.GetChild(0);
        var ability = Instantiate(upgrades[0].spawningPrefab, armSpawnPoint.position, _camera.rotation);
        ability.tag = "PlayerAttack";
        //StartCoroutine(DestroyAbility(ability, upgrades[0].despawningTime));
        
        yield return new WaitForSeconds(0.1f);
        
        // Retract
        //float animMultiplier = -1 / 4;
        //armAnim.speed *= animMultiplier;
        armAnim.Play("Retract");
        
        yield return new WaitForSeconds(_anim.GetCurrentAnimatorStateInfo(0).length); //_anim.GetCurrentAnimatorStateInfo(0).length / Mathf.Abs(animMultiplier)
        
        // Finished
        //armAnim.speed = 1;
        
        if (isRight) _canRightPunch = true;
        else _canLeftPunch = true;
    }

    // Destroy Projectile/Damagebox
    private IEnumerator DestroyAbility(GameObject obj, float destroyTime)
    {
        yield return new WaitForSeconds(destroyTime);
        Destroy(obj);
    }
    

    private void FixedUpdate()
    {
        UpdatePhysicsMovement();
    }

    private void UpdatePhysicsMovement()
    {
        // No slipping on slopes
        if (Grounded() && _rb.linearVelocity.y < 0) _rb.linearVelocity = new Vector3(0, 0, 0);


        Grounded(out var hit);
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal); //print(rotation.eulerAngles);
        
        
        var rightMovement = transform.right * _movementVector.x;
        var forwardMovement = transform.forward * _movementVector.y;
        var xyMovement = (rotation *
                          rightMovement + rotation *
                          forwardMovement).normalized * _movementSpeed;
        
        //Vector3 slopeDirection = Vector3.ProjectOnPlane(xyMovement, hit.normal);
        
        //var gravity = -9.81f;
        
        //if (!Grounded()) slopeDirection.y += gravity * Time.fixedDeltaTime; else slopeDirection.y = 0;
        
        //_rb.linearVelocity = slopeDirection;
        _rb.linearVelocity = new Vector3(xyMovement.x,  _rb.linearVelocity.y, xyMovement.z);
    }
    
    #region Camera Look
    
    private Transform _camera;
    private Vector2 _currentCursorDelta = Vector2.zero;
    private Vector2 _currentCursorDeltaVelocity = Vector2.zero;
    [SerializeField] private float _cursorSensitivity = 3.5f;
    private float _cursorSmoothTime = 0.03f;
    private float _cameraPitch;

    
    public void UpdateLookDirection(Vector2 lookDirection, bool affectYaw)
    {
        _currentCursorDelta = Vector2.SmoothDamp(_currentCursorDelta /*current position*/, 
            lookDirection /* Target Position*/, ref _currentCursorDeltaVelocity, 
            _cursorSmoothTime/*Time it takes to reach target*/);
      
      
        if (affectYaw)
        {
            #region Camera Up Down Rotation
            _cameraPitch -= _currentCursorDelta.y * _cursorSensitivity;
            _cameraPitch = Mathf.Clamp(_cameraPitch, -80f, 80f);
            _camera.localEulerAngles = Vector3.right * _cameraPitch;
            #endregion
        }
      
      
        transform.Rotate(Vector3.up * (_currentCursorDelta.x * _cursorSensitivity));
    }
    
    #endregion

    private bool Grounded()
    {
        //print("Is Grounded");
        Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.1f, LayerMask.GetMask("Ground"));
        
        return hit.collider;
    }
    private bool Grounded(out RaycastHit hit)
    {
        //print("Is Grounded");
        Physics.Raycast(transform.position, Vector3.down, out RaycastHit _hit, 1.1f, LayerMask.GetMask("Ground"));
        
        hit = _hit;
        
        return _hit.collider;
    }


    [SerializeField] private HitEffects effectsScrub;
    
    private void OnTriggerEnter(Collider other)
    {
        print("Trigger entered");

        if (other.CompareTag("DamageBox") && other.transform.parent.CompareTag("EnemyAttack"))
        {
            TakeDamage(other.GetComponent<Damage>().damageValue);
            
            print("Damage Entered");
            LingeringElements hitEffect = 0;
            
            switch (other.gameObject.layer)
            {
                case 10: // Lightning
                    print("Lightning Hit");
                    hitEffect = LingeringElements.Lightning;
                    break;
                case 11: // Water
                    print("Water Hit");
                    hitEffect = LingeringElements.Water;
                    break;
                case 12: // Fire
                    print("Fire Hit");
                    hitEffect = LingeringElements.Fire;
                    break;
                case 13: // Ice
                    print("Ice Hit");
                    hitEffect = LingeringElements.Ice;
                    break;
                case 14: // Crush
                    Debug.LogError("NOT IMPLEMENTED CRUSH!");
                    print("Crush Hit");
                    //hitEffect = 5;
                    break;
                case 15: // Pierce
                    Debug.LogError("NOT IMPLEMENTED PIERCE!");
                    print("Pierce Hit");
                    //hitEffect = 6;
                    break;
                default:
                    print("Not elemental");
                    hitEffect = LingeringElements.None;
                    break;
            }
            
            SpawnHitEffect(hitEffect);
        }
    }
    
    private void SpawnHitEffect(LingeringElements effectNum)
    {
        // Defaults to 0
        var hitEffect = effectsScrub.hitEffects[0];
        
        //if (effectsScrub.hitEffects[(int)effectNum] != null) hitEffect = effectsScrub.hitEffects[(int)effectNum];
        
        SpawnEffect(hitEffect, .5f);
    }
    
    private void SpawnEffect(GameObject effect, float time)
    {
        GameObject spawnedEffect =  Instantiate(effect, _effectSpawn.position, _effectSpawn.rotation, _effectSpawn);
        spawnedEffect.transform.localPosition += new Vector3(0, 0, 1);
        
        StartCoroutine(DestroyEffect(spawnedEffect, time));
    }
    
    private IEnumerator DestroyEffect(GameObject effect, float time)
    {
        yield return new WaitForSeconds(time);
        if (effect)
        {
            Destroy(effect);
        }
    }

    private void TakeDamage(float damage)
    {
        currentHealth -= damage;
    }
}