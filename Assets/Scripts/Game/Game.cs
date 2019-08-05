using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.InputSystem;
using Game.Player;
using UnityEngine;

namespace Game
{
    public class Game : MonoBehaviour
    {
        [SerializeField]
        private InputHandler _inputHandler;
        [SerializeField]
        private PlayerController _player;

        public void Start()
        {
            _inputHandler.Register(RegistrationFactory.Create(InputNames.A_Button,new InteractCommand(_player)));

            _inputHandler.Register(RegistrationFactory.Create(InputNames.B_Button, new JumpCommand(_player)));

            _inputHandler.Register(RegistrationFactory.Create(InputNames.LeftJoystick,new MoveCommand(_player)));
        }
    }  
}
