using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Enemy;
using Game.GamePlay;
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

        private int _currentLevel = 1;
        private List<int> _enemiesInLevels = new List<int>();
        private int _killedEnemies;

        [SerializeField] private List<GameObject> _levelHead = new List<GameObject>();
        /*[SerializeField] private GameObject _level1;
        [SerializeField] private GameObject _level2;
        [SerializeField] private GameObject _level3;

        private List<GameObject> _enemiesInLevel1 = new List<GameObject>();
        private List<GameObject> _enemiesInLevel2 = new List<GameObject>();
        private List<GameObject> _enemiesInLevel3 = new List<GameObject>();*/

        public void Start()
        {
            Instance = this;
            RegisterInputs();
            
            GetEnemiesInLevels();
            Debug.Log(_enemiesInLevels[0]);
        }

        private void Update()
        {
            if(_killedEnemies>=_enemiesInLevels[_currentLevel-1] && _currentLevel!=_levelHead.Count)
                MoveToNextLevel();
            Debug.Log(_killedEnemies);
        }

        private void GetEnemiesInLevels()
        {
            for (int i = 0; i < _levelHead.Count; i++)
            {
                _enemiesInLevels.Add(_levelHead[i].GetComponentsInChildren<BaseEnemyBehaviour>().Length);
            }
        }
        private void RegisterInputs()
        {
            _inputHandler.Register(RegistrationFactory.Create(InputNames.A_Button, new InteractACommand(_player)));

            _inputHandler.Register(RegistrationFactory.Create(InputNames.B_Button, new InteractBCommand(_player)));

            _inputHandler.Register(RegistrationFactory.Create(InputNames.X_Button, new InteractXCommand(_player)));

            _inputHandler.Register(RegistrationFactory.Create(InputNames.Y_Button, new InteractYCommand(_player)));

            _inputHandler.Register(RegistrationFactory.Create(InputNames.LeftJoystick, new MoveCommand(_player)));
        }


        private void MoveToNextLevel()
        {
            Debug.Log("Moving To NextLevel");
            _currentLevel += 1;
            _levelHead[_currentLevel - 1].SetActive(true);
            _killedEnemies = 0;
        }

        public void EnemyHasDied()
        {
            _killedEnemies += 1;
        }

    }  
}
