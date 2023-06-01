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
using Fusion.Sockets;
using Player;
using UnityEngine.Serialization;
using Core;

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
    private CameraHandler _cameraHandler;
    private MouseLook _mouseLook;
    private NetworkContainer _container;
    private NetworkRunner _runner;

    private Rigidbody _rb;
    private Rigidbody Rb {
        get {
            if (!_rb) _rb = GetComponent<NetworkRigidbody>().Rigidbody;
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
        _cameraHandler = FindObjectOfType<CameraHandler>();
        _mouseLook     = FindObjectOfType<MouseLook>();

        _mouseLook.orientation = orientation;
        body.SetActive(false);

        _defaultHeight = Col.height;
        _currentStamina = maxStamina;
        _currentSprint = 1;
    }

    private void Start() {
        _cameraHandler.StartFollowing();
    }
 
    private void Update() {
        _cBox = Col.bounds;
        isGrounded = CheckGround(groundRadius);
        if (GetInput(out NetworkInputData data)) {
            _moveVector = orientation.right * data.direction.x + orientation.forward * data.direction.y;
        }
        Rb.drag = isGrounded ? groundFriction : airFriction;
        canJump = isGrounded;
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

    public override void FixedUpdateNetwork() {
        base.FixedUpdateNetwork();
        ApplyGravity();
        switch (_currentState) {
            case PlayerState.Idle:
                _mouseLook.TransitionFOV(defaultFOV);
                break;
            case PlayerState.Walking:
                _currentSprint = 1.0f;
                _mouseLook.TransitionFOV(defaultFOV);
                Walk(_moveVector);
                break;
            case PlayerState.Running:
                _currentSprint = sprintModifier;
                _mouseLook.TransitionFOV(sprintFOV);
                Walk(_moveVector);
                break;
            case PlayerState.Sliding:
                _mouseLook.TransitionFOV(slideFOV);
                Slide(_slideVector);
                break;
            case PlayerState.Crouching:
                break;
        }
    }

    private void FixedUpdate() {
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
        Vector3 moveForce = new (moveInput.x * acceleration, 0, moveInput.z * acceleration);
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

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) {
        if (runner.IsServer) {
            
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) {
        throw new NotImplementedException();
    }

    public void OnInput(NetworkRunner runner, NetworkInput input) {
        input.Set(new NetworkInputData() {
            direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"))
        });
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) {
        throw new NotImplementedException();
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) {
        throw new NotImplementedException();
    }

    public void OnConnectedToServer(NetworkRunner runner) {
        throw new NotImplementedException();
    }

    public void OnDisconnectedFromServer(NetworkRunner runner) {
        throw new NotImplementedException();
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) {
        throw new NotImplementedException();
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) {
        throw new NotImplementedException();
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) {
        throw new NotImplementedException();
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) {
        throw new NotImplementedException();
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) {
        throw new NotImplementedException();
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) {
        throw new NotImplementedException();
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) {
        throw new NotImplementedException();
    }

    public void OnSceneLoadDone(NetworkRunner runner) {
        throw new NotImplementedException();
    }

    public void OnSceneLoadStart(NetworkRunner runner) {
        throw new NotImplementedException();
    }
}
