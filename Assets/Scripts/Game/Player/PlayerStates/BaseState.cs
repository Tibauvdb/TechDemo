using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState 
{
    public abstract void OnStateEnter();


    public abstract void OnStateExit();


    public abstract void Update();

}
