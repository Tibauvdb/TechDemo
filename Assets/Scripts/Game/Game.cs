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
        public static Game Instance;
        [SerializeField]
        private InputHandler _inputHandler;
        [SerializeField]
        private PlayerController _player;


        public List<GameObject> DamagedEnemies = new List<GameObject>();

        public void Start()
        {
            Instance = this;
            RegisterInputs();
        }

        private void RegisterInputs()
        {
            _inputHandler.Register(RegistrationFactory.Create(InputNames.A_Button, new InteractACommand(_player)));

            _inputHandler.Register(RegistrationFactory.Create(InputNames.B_Button, new InteractBCommand(_player)));

            _inputHandler.Register(RegistrationFactory.Create(InputNames.X_Button, new InteractXCommand(_player)));

            _inputHandler.Register(RegistrationFactory.Create(InputNames.Y_Button, new InteractYCommand(_player)));

            _inputHandler.Register(RegistrationFactory.Create(InputNames.LeftJoystick, new MoveCommand(_player)));
        }
    }  
}
