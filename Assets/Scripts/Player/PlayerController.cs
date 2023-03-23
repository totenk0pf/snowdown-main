/*--------------------------------------
Author: Huu Quang Nguyen
+---------------------------------------
Last modified by: Huu Quang Nguyen
--------------------------------------*/

using System;
using UnityEngine;
using Fusion;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Player states.
/// </summary>
public enum PlayerState {
    Idle,
    Walking,
    Running,
    Sliding,
    Crouching
}

/// <summary>
/// Script responsible for player movement.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : NetworkBehaviour
{
    #region Fields
    [Header("Player settings")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float groundMargin;
    [SerializeField] private float groundFriction;
    [SerializeField] private float groundRadius;
    [SerializeField] private float airFriction;
    [SerializeField] private float jumpForce;
    [SerializeField] private float slideSpeed;
    [SerializeField] private float maxStamina;
    [SerializeField] private float heightSpeed;
    [SerializeField] private float sprintModifier;
    private float _currentStamina;
    private float _defaultHeight;
    private float _currentSprint;
    
    [Header("Gravity")]
    [SerializeField] private float worldGravity;
    [SerializeField] private float gravityForce;
    [SerializeField] private LayerMask whatIsGround;
    
    [Header("Camera settings")]
    public Transform orientation;
    public Transform camTransform;
    [SerializeField] private float defaultFOV;
    [SerializeField] private float sprintFOV;
    [SerializeField] private float slideFOV;
    
    [Header("States")]
    public bool isGrounded;
    public bool canJump;
    private PlayerState _currentState;  
    private Vector3 _moveVector;
    private Vector3 _slideVector;

    [Header("Components")] 
    [SerializeField] private GameObject body;
    public static PlayerController LocalPlayerInstance;
    private MoveCamera _moveCamera;
    public MoveCamera MoveCamera {
        get {
            if (!_moveCamera) _moveCamera = FindObjectOfType<MoveCamera>();
            return _moveCamera;
        }
    }
    private MouseLook _mouseLook;

    public MouseLook MouseLookComponent {
        get {
            if (!_mouseLook) _mouseLook = FindObjectOfType<MouseLook>();
            return _mouseLook;
        }
    }
    private Rigidbody Rb => GetComponent<Rigidbody>();
    private CapsuleCollider Col => GetComponent<CapsuleCollider>();
    private Bounds _cBox;
    #endregion

    #region Unity Methods
    private void Awake() {
        if (Object.HasInputAuthority) {
            LocalPlayerInstance = this;
            MouseLookComponent.orientation = orientation;
            body.SetActive(false);
        }
        DontDestroyOnLoad(this.gameObject);
        _defaultHeight = Col.height;
        _currentStamina = maxStamina;
        _currentSprint = 1;
    }

    private void Start() {
        MoveCamera.StartFollowing();
    }
 
    private void Update() {
        _cBox = Col.bounds;
        isGrounded = CheckGround(groundRadius);
        _moveVector = orientation.right * Input.GetAxisRaw("Horizontal") + orientation.forward * Input.GetAxisRaw("Vertical");
        Rb.drag = isGrounded ? groundFriction : airFriction;
        canJump = isGrounded;
        // We switch states based on certain conditions
        if (_moveVector.magnitude > 0.1f) {
            if (Input.GetKeyDown(KeyCode.LeftControl)) _slideVector = orientation.forward.normalized;
            if (Input.GetKey(KeyCode.LeftShift)) _currentState = PlayerState.Running;
            if (Input.GetKey(KeyCode.LeftControl)) {
                _currentState = PlayerState.Sliding;
            }
            if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftControl)) _currentState = PlayerState.Walking;
        } else _currentState = PlayerState.Idle;
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (!canJump) return;
            Jump();
        }
        if (_currentState != PlayerState.Sliding) {
            ResetHeight();
            ResetStamina();
        } else {
            DecreaseHeight();
            DecreaseStamina();
        }
    }

    private void FixedUpdate() {
        ApplyGravity();
        switch (_currentState) {
            case PlayerState.Idle:
                MouseLookComponent.TransitionFOV(defaultFOV);
                break;
            case PlayerState.Walking:
                _currentSprint = 1.0f;
                MouseLookComponent.TransitionFOV(defaultFOV);
                Walk(_moveVector);
                break;
            case PlayerState.Running:
                _currentSprint = sprintModifier;
                MouseLookComponent.TransitionFOV(sprintFOV);
                Walk(_moveVector);
                break;
            case PlayerState.Sliding:
                MouseLookComponent.TransitionFOV(slideFOV);
                Slide(_slideVector);
                break;
            case PlayerState.Crouching:
                break;
        }
    }
    #endregion

    /// <summary>
    /// Check if the player is currently grounded.
    /// </summary>
    /// <param name="radius">Radius of ground contact.</param>
    /// <param name="offset">Offset of contact point.</param>
    /// <returns></returns>
    private bool CheckGround(float radius, Vector3 offset = default(Vector3)) {
        return Physics.CheckCapsule(_cBox.center, 
                                    new Vector3(_cBox.center.x + offset.x, 
                                                    _cBox.min.y - groundMargin + offset.y, 
                                                    _cBox.center.z + offset.z), 
                                    radius, 
                                    whatIsGround);
    }

    private void Jump() {
        Rb.AddRelativeForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    
    /// <summary>
    /// Walk.
    /// </summary>
    /// <param name="moveInput">Vector of current input.</param>
    private void Walk(Vector3 moveInput) {
        moveInput = moveInput.normalized;
        Vector3 moveForce = new Vector3(moveInput.x * acceleration, 0, moveInput.z * acceleration);
        if (Rb.velocity.magnitude >= maxSpeed) return;
        Rb.AddForce(moveForce * _currentSprint * Time.fixedDeltaTime, ForceMode.Acceleration);
    }

    /// <summary>
    /// Manually apply gravity since Unity's air drag is fucked.
    /// </summary>
    private void ApplyGravity() {
        if (Rb.velocity.y <= -worldGravity) return;
        Rb.AddForce(Vector3.down * gravityForce * Time.fixedDeltaTime, ForceMode.Acceleration);
    }

    private void Slide(Vector3 slideDirection) {
        Rb.AddRelativeForce(slideDirection * slideSpeed * _currentStamina, ForceMode.Acceleration);
    }

    // public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
    //     if (stream.IsWriting) {
    //         // stream.SendNext();
    //     } else {
    //         //
    //     }
    // }

    private void DecreaseHeight() {
        if (Col.height > 0) {
            Col.height /= heightSpeed;
        } else {
            Col.height = 0;
        }
    }

    private void ResetHeight() {
        if (Col.height < _defaultHeight) {
            Col.height += heightSpeed * Time.fixedDeltaTime;
        } else {
            Col.height = _defaultHeight;
        }
    }

    private void DecreaseStamina() {
        if (_currentStamina <= 0) {
            _currentStamina = 0;
            return;
        }
        _currentStamina -= Time.fixedDeltaTime;
    }

    private void ResetStamina() {
        if (_currentStamina >= maxStamina) {
            _currentStamina = maxStamina;
            return;
        }
        _currentStamina += Time.fixedDeltaTime;
    }
}
