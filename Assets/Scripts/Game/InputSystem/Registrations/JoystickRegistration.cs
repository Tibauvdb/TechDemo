using UnityEngine;

namespace Game.InputSystem
{
    public class JoystickRegistration : Registration
    {
        private readonly IDirectionCommand _command;


        private readonly string _inputNameHorizontal;
        private readonly string _inputNameVertical;

        public JoystickRegistration(string inputName, IDirectionCommand command)
        {
            _command = command;

            _inputNameHorizontal = $"{inputName}_Horizontal";
            _inputNameVertical = $"{inputName}_Vertical";
        }

        public void Update()
        {
            float h = Input.GetAxis(_inputNameHorizontal);
            float v = Input.GetAxis(_inputNameVertical);

            Vector2 direction = new Vector2(h, v);

            _command.Execute(direction);
        }

    }
}
