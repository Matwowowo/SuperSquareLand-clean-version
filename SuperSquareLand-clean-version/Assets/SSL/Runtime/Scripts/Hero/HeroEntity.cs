using UnityEngine;
using UnityEngine.Serialization;

public class HeroEntity : MonoBehaviour
{
    [Header("Physics")]
    [SerializeField] private Rigidbody2D _rigidbody;

    [Header("Horizontal Movements")]
    [FormerlySerializedAs("_movementSettings")]
    [SerializeField] private HeroHorizontalMovementSettings _groundHorizontalMovementSettings;
    [SerializeField] private HeroHorizontalMovementSettings _airHorizontalMovementSettings;
    private float _horizontalSpeed = 0f;
    private float _moveDirX = 0f;



    [Header("Dash")]
    [SerializeField] private HeroDashSettings _dashSettings;
    private float _dashTimer;
    
    

    [Header("Orientation")]
    [SerializeField] private Transform _orientVisualRoot;
    private float _orientX = 1f;

    [Header("Debug")]
    [SerializeField] private bool _guiDebug = false;

    [Header("Vertical Movement")]
    private float _verticalSpeed = 0f;

    [Header("Fall")]
    [SerializeField] private HeroFallSettings _fallSettings;

    [Header("Ground")]
    [SerializeField] private GroundDetector _groundDetector;

    [Header("Jump")]
    [SerializeField] private HeroJumpSettings _jumpSettings;
    [SerializeField] private HeroFallSettings _jumpFallSettings;

    public bool IsTouchingGround { get; private set; } = false;


    public void SetMoveDirX(float dirX)
    {
        _moveDirX = dirX;  
    }

    #region Jump
    enum JumpState
    {
        NotJumping,
        JumpImpulsion,
        Falling
    }
    
    private JumpState _jumpState = JumpState.NotJumping;
    public bool IsJumping => _jumpState != JumpState.NotJumping;
    private float _jumpTimer;
    
    public void JumpStart () 
    {
        _jumpState = JumpState.JumpImpulsion;
        _jumpTimer = 0f;
    }    

    private void _UpdateJumpStateImpulsion()
    {
        _jumpTimer += Time.fixedDeltaTime;
        if (_jumpTimer < _jumpSettings.jumpMaxDuration) {
            _verticalSpeed = _jumpSettings.jumpSpeed;
        
        } else
        {
            _jumpState = JumpState.Falling;
        }
    }

    private void _UpdateJumpStateFalling() 
    { 
        if(!IsTouchingGround)
        {
            _ApplyFallGravity(_jumpFallSettings);
        } else
        {
            _ResetVerticalSpeed();
            _jumpState = JumpState.NotJumping;
        }
    }

    private void _UpdateJump()
    {
        switch (_jumpState)
        {
            case JumpState.JumpImpulsion:
                _UpdateJumpStateImpulsion();
                break;

            case JumpState.Falling:
                _UpdateJumpStateFalling();
                break;
        }
    }

    public void _StopJumpImpulsion()
    {
        _jumpState = JumpState.Falling;
    }

    public bool IsJumpImpulsing => _jumpState == JumpState.JumpImpulsion;
    public bool isJumpMinDurationReached => _jumpTimer >= _jumpSettings.jumpMinDuration;

    #endregion
    private void FixedUpdate()
    {
        _ApplyGroundDetection();


        HeroHorizontalMovementSettings horizontalMovementSettings = _GetCurrentHorizontalMovementSettings();
        if (_AreOrientAndMovementOpposite())
        {
            _TurnBack(horizontalMovementSettings);
        } else
        {
            _UpdateHorizontalSpeed(horizontalMovementSettings);
            _ChangeOrientFromHorizontalMovement();
        }

        if(IsJumping)
        {
            _UpdateJump();
        } else
        {

        
        
            if (!IsTouchingGround)
            {
                _ApplyFallGravity(_fallSettings);
            } else
            {
             _ResetVerticalSpeed();
         }
        }
        _ApplyFallGravity(_fallSettings);

        _ApplyHorizontalSpeed();
        _ApplyVerticalSpeed();
        
    
    }
    #region Orientation
    private void _ChangeOrientFromHorizontalMovement()
    {
        if (_moveDirX == 0f) return;
        _orientX = Mathf.Sign(_moveDirX);
    }

    private void _UpdateOrientVisual()
    {

        Vector3 newScale = _orientVisualRoot.localScale;
        newScale.x = _orientX;
        _orientVisualRoot.localScale = newScale;


    }

    private void _TurnBack(HeroHorizontalMovementSettings settings)
    {
        _horizontalSpeed -= settings.turnBackFrictions * Time.fixedDeltaTime;
        if (_horizontalSpeed < 0f)
        {
            _horizontalSpeed = 0f;
            _ChangeOrientFromHorizontalMovement();
        }
    }

    private bool _AreOrientAndMovementOpposite()
    {
        return _moveDirX * _orientX < 0f;
    }

    #endregion
    private void _ApplyHorizontalSpeed()
    {
        Vector2 velocity = _rigidbody.velocity;
        velocity.x = _horizontalSpeed * _orientX;
        _rigidbody.velocity = velocity;
    }

    private HeroHorizontalMovementSettings _GetCurrentHorizontalMovementSettings()
    {
        return IsTouchingGround ? _groundHorizontalMovementSettings : _airHorizontalMovementSettings;

    }
    
    private void Update()
    {
        _UpdateOrientVisual();
    }

    
    private void _ApplyGroundDetection()
    {
        IsTouchingGround = _groundDetector.DetectGroundNearBy();
    }
    
    private void _ResetVerticalSpeed()
    {
        _verticalSpeed = 0f;
    }
    private void _ApplyFallGravity(HeroFallSettings settings)
    {
        _verticalSpeed -= settings.fallGravity * Time.fixedDeltaTime;
        if (_verticalSpeed < -settings.fallSpeedMax)
        {
            _verticalSpeed = -settings.fallSpeedMax;
        }
    }

    private void _ApplyVerticalSpeed()
    {
        Vector2 velocity = _rigidbody.velocity;
        velocity.y = _verticalSpeed;
        _rigidbody.velocity = velocity;
    }

    #region Horizontal
    private void _UpdateHorizontalSpeed(HeroHorizontalMovementSettings settings)
    {
        if (_moveDirX != 0f)
        {
            _Accelerate(settings);
        } else
        {
            _Decelerate(settings);
        }
    }

    private void _Accelerate(HeroHorizontalMovementSettings settings)
    {
        _horizontalSpeed += settings.acceleration * Time.fixedDeltaTime;
        if (_horizontalSpeed > settings.speedMax)
        {
            _horizontalSpeed = settings.speedMax;
        }
    }

    private void _Decelerate(HeroHorizontalMovementSettings settings)
    {
        _horizontalSpeed -= settings.deceleration * Time.fixedDeltaTime;
        if (_horizontalSpeed  < 0f)
        {
            _horizontalSpeed = 0f;
        }
    }

    #endregion
    public void DashStart(HeroHorizontalMovementSettings settings) /* Faire en 2 fonctions disctincts*/
    {
        _dashTimer = 0f;
        _dashTimer += Time.deltaTime;
        if ( _dashTimer < _dashSettings.duration) 
        { 
            _horizontalSpeed = _dashSettings.speed;
        } else
        {
            _horizontalSpeed = settings.speedMax;
        }

    }
    private void OnGUI()
    {
        if (!_guiDebug) return;

        GUILayout.BeginVertical(GUI.skin.box);
        GUILayout.Label(gameObject.name);
        GUILayout.Label($"MoveDirX = {_moveDirX}");
        GUILayout.Label($"OrientX = {_orientX}");
        if (IsTouchingGround)
        {
            GUILayout.Label("OnGround");
        } else
        {
            GUILayout.Label("In air");
        }
        GUILayout.Label($"Jump State = {_jumpState}");
        GUILayout.Label($"Horizontal Speed = {_horizontalSpeed}");
        GUILayout.Label($"Vertical Speed = {_verticalSpeed}");
        GUILayout.EndVertical();
    }
}