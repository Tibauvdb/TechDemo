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
        ;
    }

    public override void Update()
    {
        
    }
}
