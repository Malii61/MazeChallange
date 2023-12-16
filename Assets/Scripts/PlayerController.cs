using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.XR;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _sprintSpeed, _walkSpeed, _smoothTime, _mouseSensitivity, _jumpForce;
    [SerializeField] private PlayerAnimator _playerAnimator;
    private bool _isGrounded;
    private Vector3 smoothMoveVelocity;
    private Vector3 moveAmount;
    [Tooltip("What layers the character uses as ground")]
    [SerializeField] LayerMask _groundLayers;
    [SerializeField] Transform _playerModel;
    [SerializeField] private CinemachineVirtualCamera fpsCam;
    [SerializeField] private CinemachineVirtualCamera tpsCam;
    [SerializeField] private Camera cam;
    private Vector3 stopPl = Vector3.zero;
    private Rigidbody _rigidbody;
    [SerializeField] private GameObject _cinemachineCameraTarget;
    private float _speed;
    [SerializeField] private float _speedChangingSmoothness = 5f;
    [SerializeField] private Animator _animator;
    [SerializeField] private GroundChecker groundChecker;
    private Vector3 spawnPosition;
    //tps cam rotation
    private float _cinemachineTargetYaw = 0;
    private float _cinemachineTargetPitch = 0;
    private float TopClamp = 70.0f;
    private float BottomClamp = -30.0f;
    private float CameraAngleOverride = 0.0f;

    // animation IDs
    private int _animIDGrounded;
    private int _animIDJump;
    private bool isJumped;
    private void Awake()
    {
        spawnPosition = transform.position;
        _rigidbody = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        if (cam == null)
            cam = Camera.main;
        AssignAnimationIDs();
        GameInput.Instance.OnChangeCam += GameInput_OnChangeCam;
    }
    private void Update()
    {
        Jump();
    }
    private void FixedUpdate()
    {
        Move();
    }
    private void LateUpdate()
    {
        CameraRotation();
    }
    private void GameInput_OnChangeCam(object sender, System.EventArgs e)
    {
        if (EventSystem.current.currentSelectedGameObject)
            return;

        else if (GameInput.Instance.GetInputActions().Player.ChangeCam.triggered)
        {
            fpsCam.enabled = !fpsCam.isActiveAndEnabled;
            tpsCam.enabled = !tpsCam.isActiveAndEnabled;
            int layer = tpsCam.enabled ? LayerFinder.GetIndex(Layer.Default) : LayerFinder.GetIndex(Layer.Unvisible);
            Utils.SetRenderLayerInChildren(_playerModel, layer);
        }
    }
    private void AssignAnimationIDs()
    {
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
    }
    private void Move()
    {
        if (EventSystem.current.currentSelectedGameObject)
        {
            moveAmount = stopPl;
            //_animator.SetFloat("Speed", 0);
            return;
        }

        Vector2 movement = GameInput.Instance.GetMovementVectorNormalized();


        Vector3 forward = cam.transform.forward;
        Vector3 right = cam.transform.right;

        forward.y = 0f; // Y ekseni üzerindeki dönüþü önlemek için sýfýrlayýn.
        right.y = 0f;

        Vector3 moveDir = (forward * movement.y + right * movement.x).normalized;

        // Yerçekimi etkisi olmasýn
        moveDir.y = 0f;

        if (GameInput.Instance.GetInputActions().Player.Sprint.IsPressed())
        {
            moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * _sprintSpeed, ref smoothMoveVelocity, _smoothTime);
        }
        else
        {
            moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * _walkSpeed, ref smoothMoveVelocity, _smoothTime);
        }
        _speed = Mathf.Lerp(_speed, moveAmount.magnitude, Time.fixedDeltaTime * _speedChangingSmoothness);
        _animator.SetFloat("Speed", _speed);
        //Rotation
        if (tpsCam.isActiveAndEnabled)
        {
            if (movement != Vector2.zero)
            {
                float targetAngle = Mathf.Atan2(movement.x, movement.y) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
                Quaternion rotation = Quaternion.Euler(0f, targetAngle, 0f);
                _playerModel.transform.rotation = Quaternion.Lerp(_playerModel.transform.rotation, rotation, Time.deltaTime * 4f);
            }
        }
        else if (fpsCam.isActiveAndEnabled)
        {
            Quaternion rotation = Quaternion.Euler(0f, cam.transform.eulerAngles.y, 0f);
            _playerModel.transform.rotation = Quaternion.Lerp(_playerModel.transform.rotation, rotation, Time.deltaTime * 4f);
        }
        _rigidbody.MovePosition(transform.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);

        CheckFallRespawn();
    }
    private void CheckFallRespawn()
    {
        if (transform.position.y < -12)
        {
            Teleport(spawnPosition);
        }
    }
    public void Teleport(Vector3 pos, Quaternion rot = default)
    {
        if (rot == default)
            rot = Quaternion.identity;
        transform.position = pos;
        _playerModel.transform.rotation = rot;
    }
    private void CameraRotation()
    {
        _cinemachineTargetYaw += GameInput.Instance.GetMouseLook().x * _mouseSensitivity;
        _cinemachineTargetPitch += -GameInput.Instance.GetMouseLook().y * _mouseSensitivity;
        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);
        _cinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
            _cinemachineTargetYaw, Time.deltaTime * 4f);

    }
    private void Jump()
    {
        _isGrounded = groundChecker.isGrounded;
        if (_isGrounded)
        {
            // Jump
            if (GameInput.Instance.IsJumpButtonPressed())
            {
                isJumped = true;
                _rigidbody.AddForce(transform.up * _jumpForce, ForceMode.Impulse);

                _animator.SetBool(_animIDJump, true);
            }
            else if (isJumped)
            {
                _animator.SetBool(_animIDJump, false);
                isJumped = false;
            }
        }
        _animator.SetBool(_animIDGrounded, _isGrounded);
    }
    private float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
