using System;
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
    
    private float _lookSensitivity;
    private float _movementSpeed = 5f;
    private float _jumpForce;
    private float _health;
    
    
    
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        
        _camera = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateInputs();
        UpdateLookDirection(_lookVector, true);
    }

    private void FixedUpdate()
    {
        UpdatePhysicsMovement();
    }

    private void UpdatePhysicsMovement()
    {
        var rightMovement = transform.right * _movementVector.x;
        var forwardMovement = transform.forward * _movementVector.y;
        
        _rb.linearVelocity = (rightMovement + forwardMovement).normalized * _movementSpeed;
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
}
