using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]

public class HeroHorizontalMovementSettings
{
    public float acceleration = 20f;
    public float deceleration = 20f;   
    public float speedMax = 5f;
    public float turnBackFrictions = 25f;
}

