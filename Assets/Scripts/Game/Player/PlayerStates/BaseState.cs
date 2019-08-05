﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState 
{
    public abstract void OnStateEnter();


    public abstract void OnStateExit();


    public abstract void Update();


    public abstract void Move(Vector2 direction);
    public abstract void InteractA();
    public abstract void InteractB();
    public abstract void InteractX();
    public abstract void InteractY();
    
}
