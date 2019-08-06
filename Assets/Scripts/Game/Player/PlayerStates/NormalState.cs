using System.Collections;
using System.Collections.Generic;
using Game.Player;
using UnityEngine;

public class NormalState : BaseState
{
    private readonly PlayerMotor _playerMotor;
    private readonly PlayerController _playerController;

    public NormalState(PlayerMotor playerMotor, PlayerController playerController)
    {
        _playerMotor = playerMotor;
        _playerController = playerController;
    }

    // Update is called once per frame
    public override void OnStateEnter()
    {
        
    }

    public override void OnStateExit()
    {
        
    }

    public override void Update()
    {

    }

    public override void Move(Vector2 direction)
    {
        _playerMotor.IsWalking = CheckIfWalking(direction);
        //Debug.Log(direction.x + " | " + Input.GetAxis("LeftJoystick_Horizontal"));

        if (_playerMotor.IsGrounded)
        {
            _playerMotor.IsWalking = CheckIfWalking(direction);

            _playerMotor.Movement = new Vector3(direction.x,0,direction.y);
        }
    }

    private bool CheckIfWalking(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) < 0.1f && Mathf.Abs(direction.y) < 0.1f)
            return false;
        return true;
    }
    public override void InteractA()
    {
    }

    public override void InteractB()
    {
    }

    public override void InteractX()
    {
    }

    public override void InteractY()
    {
    }
}
