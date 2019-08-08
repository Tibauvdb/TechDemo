using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.GamePlay;

namespace Game.Player
{
    public class StateMachineController
    {
        private List<BaseState> _states = new List<BaseState>();

        private readonly PlayerMotor _playerMotor;
        private readonly PlayerController _playerController;
        private readonly AnimationsController _animController;


        public StateMachineController(PlayerMotor playerMotor, PlayerController playerController,
            AnimationsController animController)
        {
            _playerMotor = playerMotor;
            _playerController = playerController;
            _animController = animController;

            CreateStates();
        }

        private void CreateStates()
        {
            _states.Add(new NormalState(_playerMotor,_playerController,_animController));
            _states.Add(new AttackState(_playerMotor,_playerController,_animController));
        }

        public BaseState GetState<T>(IInteractable interactableObject) where T : BaseState
        {
            foreach (BaseState state in _states)
            {
                if (state is T)
                {
                    //state.OnStateExit();
                    return state;
                }

            }

            throw new SystemException("Requested State Does Not Exist");
        }
    }
}
