using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.InputSystem
{
    public class ButtonDownRegistration : Registration
    {
        private readonly string _inputName;
        private readonly IImpulseCommand _command;

        public ButtonDownRegistration(string inputName, IImpulseCommand command)
        {
            _inputName = inputName;
            _command = command;
        }

        public void Update()
        {
            if (ShouldExecute())
                Execute();
        }

        private void Execute()
        {
            _command.Execute();
        }

        private bool ShouldExecute()
        {
            return Input.GetButtonDown(_inputName);
        }
    }
}
