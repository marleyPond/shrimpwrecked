using UnityEngine;
using System.Collections;

public class Spike : Projectile {

    new void Awake()
    {
        base.Awake();
        speed = 5f;
        targetShrimp = true;
    }


    
}
