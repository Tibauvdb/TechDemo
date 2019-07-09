using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.InputSystem
{
    public class InputHandler : MonoBehaviour
    {
        private readonly List<Registration> _registrations = new List<Registration>();

        private void Update()
        {
            foreach (var registration in _registrations)
            {
               registration.Update();
            }
        }

        public void Register(Registration registration)
        {
            _registrations.Add(registration);
        }

    }
}
