using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.InputSystem
{

    public interface IDirectionCommand
    {
        void Execute(Vector2 direction);
    }

    public interface IContinuousCommand 
    {
        void Execute(bool isDown);
    }

    public interface IImpulseCommand 
    {
        void Execute();
    }

    
}
