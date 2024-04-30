using UnityEngine;

public class HeroEntity : MonoBehaviour
{
    [Header("Physics")]
    [SerializeField] private Rigidbody2D _rigidbody;

    [Header("Horizontal Movements")]
    [SerializeField] private HeroHorizontalMovementSettings _movementSettings;
    private float _horizontalSpeed = 0f;
    private float _moveDirX = 0f;

    [Header("Dash")]
    [SerializeField] private HeroDashSettings _dashSettings;
    public float _moveDash = 0f;
    public float _dashDuration = 0f;
    

    [Header("Orientation")]
    [SerializeField] private Transform _orientVisualRoot;
    private float _orientX = 1f;

    [Header("Debug")]
    [SerializeField] private bool _guiDebug = false;

    [Header("Vertical Movement")]
    private float _verticalSpeed = 0f;

    [Header("Fall")]
    [SerializeField] private HeroFallSettings _fallSettings;

    public void SetMoveDirX(float dirX)
    {
        _moveDirX = dirX;  
    }

    public void SetDash(float dash)
    {
        _moveDash = dash;
    }

    private void FixedUpdate()
    {
        if (_AreOrientAndMovementOpposite())
        {
            _TurnBack();
        } else
        {
            _UpdateHorizontalSpeed();
            _ChangeOrientFromHorizontalMovement();
        }

        _ApplyFallGravity();

        _ApplyHorizontalSpeed();
        _ApplyVerticalSpeed();
        _Dash();
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

    private void _TurnBack()
    {
        _horizontalSpeed -= _movementSettings.turnBackFrictions * Time.fixedDeltaTime;
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
    
    private void Update()
    {
        _UpdateOrientVisual();
    }

    private void _ApplyFallGravity()
    {
        _verticalSpeed -= _fallSettings.fallGravity * Time.fixedDeltaTime;
        if (_verticalSpeed < -_fallSettings.fallSpeedMax)
        {
            _verticalSpeed = -_fallSettings.fallSpeedMax;
        }
    }

    private void _ApplyVerticalSpeed()
    {
        Vector2 velocity = _rigidbody.velocity;
        velocity.y = _verticalSpeed;
        _rigidbody.velocity = velocity;
    }

    private void _UpdateHorizontalSpeed()
    {
        if (_moveDirX != 0f)
        {
            _Accelerate();
        } else
        {
            _Decelerate();
        }
    }

    private void _Accelerate()
    {
        _horizontalSpeed += _movementSettings.acceleration * Time.fixedDeltaTime;
        if (_horizontalSpeed > _movementSettings.speedMax)
        {
            _horizontalSpeed = _movementSettings.speedMax;
        }
    }

    private void _Decelerate()
    {
        _horizontalSpeed -= _movementSettings.deceleration * Time.fixedDeltaTime;
        if (_horizontalSpeed  < 0f)
        {
            _horizontalSpeed = 0f;
        }
    }

    
    public void _Dash()
    {
        _moveDash = _dashSettings.speed * Time.fixedDeltaTime;

    }

    /* public void _DashTimer()
    {
        _dashDuration = _dashSettings.duration;
        if()
    } */
    private void OnGUI()
    {
        if (!_guiDebug) return;

        GUILayout.BeginVertical(GUI.skin.box);
        GUILayout.Label(gameObject.name);
        GUILayout.Label($"MoveDirX = {_moveDirX}");
        GUILayout.Label($"OrientX = {_orientX}");
        GUILayout.Label($"Horizontal Speed = {_horizontalSpeed}");
        GUILayout.Label($"Vertical Speed = {_verticalSpeed}");
        GUILayout.EndVertical();
    }
}