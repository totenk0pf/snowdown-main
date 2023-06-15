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
using Combat;
using Fusion.Sockets;
using Player;
using UnityEngine.Serialization;
using Core;
using Core.Events;
using Core.Logging;
using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using Unity.VisualScripting;
using EventType = Core.Events.EventType;

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
public class PlayerController : NetworkBehaviour, INetworkRunnerCallbacks
{
#region Fields
    [Header("Speed")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float acceleration;
    
    [Header("Ground")]
    [SerializeField] private float groundMargin;
    [SerializeField] private float groundFriction;
    [SerializeField] private float groundRadius;
    
    [Header("Air")]
    [SerializeField] private float airFriction;
    [SerializeField] private float jumpForce;
    
    [Header("Slide")]
    [SerializeField] private float slideSpeed;
    [SerializeField] private float slideThreshold;
    
    [Header("Sprint")]
    [SerializeField] private float sprintModifier;
    [SerializeField] private float maxStamina;
    [SerializeField] private float staminaRegen;
    [SerializeField] private float staminaCost;
    
    [Header("Crouch")]
    [SerializeField] private float heightSpeed;
    [SerializeField] private float heightResetSpeed;
    [SerializeField] private float crouchModifier;

    [Header("Step")] 
    [SerializeField] private float stepHeightOffset;
    [SerializeField] private float stepCrouchOffset;
    [SerializeField] private float stepDistance;
    [SerializeField] private Vector3 stepExtents;
    [SerializeField] private float stepForce;

    [Header("Slope")] 
    [SerializeField] private float slopeForce;
    private bool _isOnSlope;
    private Vector3 _slopeNormal;

    private float _currentStamina;
    private float _defaultHeight;
    private float _currentSprint;
    
    [Header("Gravity")]
    [SerializeField] private float gravityForce;
    [SerializeField] private float gravityLimit;
    [SerializeField] private LayerMask whatIsGround;
    private Vector3 _gravityDir;
    
    [Header("Camera settings")]
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform camTransform;
    [SerializeField] private Transform viewTransform;
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
    [SerializeField] private Canvas hud;
    private CameraHandler _cameraHandler;
    private MouseLook _mouseLook;
    private NetworkContainer _container;
    private NetworkRunner _runner;
    private NetworkObject _networkObject;

    private Rigidbody _rb;
    private Rigidbody Rb {
        get {
            if (!_rb) _rb = GetComponent<Rigidbody>();
            return _rb;
        }  
    }

    private CapsuleCollider _col;
    private CapsuleCollider Col {
        get {
            if (!_col) _col = GetComponent<CapsuleCollider>();
            return _col;
        }
    }
    
    private Bounds _cBox;
#endregion

    #region Unity Methods
    private void Awake() {
        _container     = NetworkContainer.Instance;
        _runner        = _container.runner;
        if (_runner) _runner.AddCallbacks(this);

        _networkObject = GetComponent<NetworkObject>();
        _cameraHandler = FindObjectOfType<CameraHandler>();
        _mouseLook     = _cameraHandler.mouseLook;
        if (hud) hud.worldCamera = _cameraHandler.uiCamera;

        _defaultHeight  = Col.height;
        _currentStamina = maxStamina;
        _currentSprint  = 1;
        _gravityDir     = Vector3.down;
    }

    private void OnDestroy() {
        if (_runner) return;
        _runner.RemoveCallbacks(this);
    }

    public override void Spawned() {
        if (_networkObject.HasInputAuthority) {
            body.SetActive(false);
            _cameraHandler.StartFollowing(camTransform);
            _mouseLook.orientation   = orientation;
            _mouseLook.viewTransform = viewTransform;
            _mouseLook.SetupView();
        }
    }

    private void Update() {
        _cBox       = Col.bounds;
        isGrounded  = CheckGround(groundRadius);
        Rb.drag     = isGrounded ? groundFriction : airFriction;
        if (IsOnSlope(out Vector3 normal)) {
            _isOnSlope   = true;
            _slopeNormal = normal;
            _gravityDir  = normal;
        } else {
            _isOnSlope   = false;
            _slopeNormal = Vector3.zero;
            _gravityDir  = Vector3.down;
        }
        canJump     = isGrounded;
        if (_currentState != PlayerState.Crouching) {
            ResetHeight();
        } else {
            DecreaseHeight();
        }
        if (_currentState != PlayerState.Sliding) {
            ResetStamina();
        } else {
            DecreaseStamina();
        }
    }

    public override void FixedUpdateNetwork() {
        base.FixedUpdateNetwork();
        if (!GetInput(out NetworkInputData data)) return;
        _moveVector = orientation.right * data.direction.x + orientation.forward * data.direction.y;
        if (_moveVector.magnitude > 0.1f) {
            SlopeStep();
            Step();
            this.FireEvent(EventType.OnPlayerMove);
            if (_currentState == PlayerState.Sliding) {
                if (_slideVector == Vector3.zero) _slideVector = orientation.forward.normalized;
                return;
            }
            _slideVector = Vector3.zero;
            if (data.moveInput.IsSet(InputButtons.Sprint)) {
                // _currentState = PlayerState.Running;
            }
            if (!data.moveInput.IsSet(InputButtons.Sprint) && !data.moveInput.IsSet(InputButtons.Crouch)) {
                _currentState = PlayerState.Walking;
            }
        } else {
            _currentState = PlayerState.Idle;
            this.FireEvent(EventType.OnPlayerStop);
        }
        if (data.moveInput.IsSet(InputButtons.Crouch)) {
            // if (isGrounded && Rb.velocity.magnitude >= slideThreshold) {
                // _currentState = PlayerState.Sliding;
            // }
            _currentState = PlayerState.Crouching;
        }
        if (_currentState != PlayerState.Sliding && data.moveInput.IsSet(InputButtons.Crouch)) {
            // _currentState = PlayerState.Crouching;
        }
        if (data.moveInput.IsSet(InputButtons.Jump)) {
            if (!canJump) return;
            Jump();
        }
        ApplyGravity();
        switch (_currentState) {
            case PlayerState.Idle:
                break;
            case PlayerState.Walking:
                _currentSprint = 1.0f;
                Walk(_moveVector);
                UpdateBob(1f, 1f);
                break;
            case PlayerState.Running:
                _currentSprint = sprintModifier;
                Walk(_moveVector);
                UpdateBob(sprintModifier, sprintModifier);
                break;
            case PlayerState.Sliding:
                Slide(_slideVector);
                break;
            case PlayerState.Crouching:
                _currentSprint = crouchModifier;
                UpdateBob(crouchModifier, crouchModifier);
                Walk(_moveVector);
                break;
        }
    }

    private void LateUpdate() {
        switch (_currentState) {
            case PlayerState.Idle:
                _mouseLook.TransitionFOV(defaultFOV);
                break;
            case PlayerState.Walking:
                _mouseLook.TransitionFOV(defaultFOV);
                break;
            case PlayerState.Running:
                _mouseLook.TransitionFOV(sprintFOV);
                break;
            case PlayerState.Sliding:
                _mouseLook.TransitionFOV(slideFOV);
                break;
            case PlayerState.Crouching:
                _mouseLook.TransitionFOV(defaultFOV);
                break;
        }
    }
#endregion
    
    private void Step() {
        if (!isGrounded) return;
        // if (_isOnSlope) return;
        Vector3 stepOffset = _currentState == PlayerState.Crouching ? GetStepOffset(stepCrouchOffset) : GetStepOffset(stepHeightOffset);
        Vector3 dir = (stepOffset - orientation.position).normalized;
        Debug.DrawLine(orientation.position, stepOffset, Color.green);
        if (!Physics.Raycast(orientation.position,
                             dir,
                             out RaycastHit hit,
                             Vector3.Distance(orientation.position, stepOffset),
                             whatIsGround)) return;
        Debug.DrawLine(orientation.position, hit.point, Color.yellow);
        if (hit.normal != Vector3.up) return;
        Vector3 mirroredTarget = stepOffset;
        mirroredTarget.y *= -1;
        Vector3 forceDir = (mirroredTarget - orientation.position).normalized;
        Rb.AddForce(forceDir * stepForce * _runner.DeltaTime, ForceMode.VelocityChange);
    }

    private void SlopeStep() {
        if (!_isOnSlope) return;
        Vector3 slopeDir = Vector3.Cross(orientation.right, _slopeNormal).normalized;
        Rb.AddForce(slopeDir * slopeForce * _runner.DeltaTime, ForceMode.VelocityChange);
    }

    private Vector3 GetStepOffset(float height) {
        Vector3 step = orientation.position + _moveVector.normalized * stepDistance;
        step.y += height;
        return step;
    }

    private bool IsOnSlope(out Vector3 slopeNormal) {
        Vector3 stepOffset = _currentState == PlayerState.Crouching ? GetStepOffset(stepCrouchOffset) : GetStepOffset(stepHeightOffset);
        Vector3 dir = (stepOffset - orientation.position).normalized;
        if (!Physics.Raycast(orientation.position,
                             dir,
                             out RaycastHit hit,
                             Vector3.Distance(orientation.position, stepOffset),
                             whatIsGround)) {
            slopeNormal = Vector3.zero;
            return false;
        }
        slopeNormal = hit.normal;
        var dot = Vector3.Dot(hit.normal, Vector3.up);
        return dot is > 0 and < 1;
    }
    
    private void UpdateBob(float h, float v) {
        this.FireEvent(EventType.OnWeaponBobUpdate, new WeaponBobMsg {
            horizontalMod = h,
            verticalMod   = v,
        });
    }

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
        Vector3 moveForce = new (moveInput.x * acceleration, 0, moveInput.z * acceleration);
        if (Rb.velocity.magnitude >= maxSpeed) return;
        Rb.AddForce(moveForce * _currentSprint * _runner.DeltaTime, ForceMode.Acceleration);
    }

    /// <summary>
    /// Manually apply gravity since Unity's air drag is fucked.
    /// </summary>
    private void ApplyGravity() {
        if (Rb.velocity.y <= -gravityLimit) return;
        Rb.AddForce(_gravityDir * gravityForce * _runner.DeltaTime, ForceMode.Acceleration);
    }

    private void Slide(Vector3 slideDirection) {
        Rb.AddRelativeForce(slideDirection * slideSpeed * _currentStamina, ForceMode.Acceleration);
    }

    private void DecreaseHeight() {
        if (Col.height > 0) {
            Col.height -= heightSpeed * Time.deltaTime;
        } else {
            Col.height = 0;
        }
    }

    private void ResetHeight() {
        if (Col.height == _defaultHeight) return;
        if (Col.height < _defaultHeight) {
            Col.height += heightResetSpeed * Time.deltaTime;
        } else {
            Col.height = _defaultHeight;
        }
    }

    private void DecreaseStamina() {
        if (_currentStamina <= 0) {
            _currentStamina = 0;
            return;
        }
        _currentStamina -= staminaCost * _runner.DeltaTime;
    }

    private void ResetStamina() {
        if (_currentStamina == maxStamina) return;
        if (_currentStamina >= maxStamina) {
            _currentStamina = maxStamina;
            return;
        }
        _currentStamina += staminaRegen * _runner.DeltaTime;
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Vector3 o = orientation.position + _moveVector.normalized * stepDistance;
        o.y += _currentState == PlayerState.Crouching ? stepCrouchOffset : stepHeightOffset;
        Gizmos.DrawWireCube(o, stepExtents);
        Gizmos.DrawLine(orientation.position, o);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(new Vector3(_cBox.center.x, 
                                        _cBox.min.y - groundMargin, 
                                        _cBox.center.z), groundRadius);
    }
#endif

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
}
