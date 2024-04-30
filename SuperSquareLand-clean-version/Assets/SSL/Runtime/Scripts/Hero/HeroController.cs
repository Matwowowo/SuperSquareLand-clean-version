using UnityEngine;

public class HeroController : MonoBehaviour
{
    [Header("Entity")]
    [SerializeField] private HeroEntity _entity;

    [Header("Debug")]
    [SerializeField] private bool _guiDebug = false;


    private void Update()
    {
        _entity.SetMoveDirX(GetInputMoveX());
        if (_GetInputDownJump())
        {
            if (_entity.IsTouchingGround && !_entity.IsJumping) 
            { 
                _entity.JumpStart();
            }
        }
    
        if (_entity.IsJumpImpulsing) 
        {
            if (!_GetInputJump() && _entity.isJumpMinDurationReached)
            {
                _entity._StopJumpImpulsion();
            }
        }
    }

    private bool _GetInputJump()
    {
        return Input.GetKey(KeyCode.Space);
    }
    private bool _GetInputDownJump()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    private float GetInputMoveX()
    {
        float inputMoveX = 0f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.Q)) {

            inputMoveX = -1f;
        }
    
        if (Input.GetKey(KeyCode.D))
        {
            inputMoveX = 1f;
        }
    
        return inputMoveX;
    }

    private float GetInputDash()
    {
        float inputDash = 0f;
        if(Input.GetKey(KeyCode.E))
        {
            inputDash = _entity._moveDash;
        }

        return inputDash;
    }
    private void OnGUI()
    {
        if (!_guiDebug) return;

        GUILayout.BeginVertical(GUI.skin.box);
        GUILayout.Label(gameObject.name);
        GUILayout.EndVertical();
    }
}