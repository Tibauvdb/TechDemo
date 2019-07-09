using UnityEngine;

namespace Game.InputSystem
{
    public class ButtonRegistration : Registration
    {
        private string _inputName;
        private IContinuousCommand _command;

        private bool _previousState;
        private bool _currentState;

        public ButtonRegistration(string inputName, IContinuousCommand command)
        {
            _inputName = inputName;
            _command = command;
        }

        public void Update()
        {
            UpdateState();
            if (ShouldExecute())
                Execute();
        }

        private void Execute()
        {
            _command.Execute(_currentState);
        }

        private void UpdateState()
        {
            _previousState = _currentState;

            _currentState = Input.GetButton(_inputName);
        }

        private bool ShouldExecute()
        {
            return _currentState || (_previousState && !_currentState);
        }
    }
}
