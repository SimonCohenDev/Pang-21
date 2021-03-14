using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class Collidable : MonoBehaviour
{
    protected GameManager gameManager;
    public virtual void Init(EntityData data) 
    {
        initialize();
    }
    public virtual void Init()
    {
        initialize();
    }
    private void initialize ()
    {
        gameManager = GameManager.Instance;
    }
}
