using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Weapon 
{
    public WeaponObject prefab;
    public Vector2 SpawningPoint;
    public float FireCooldown = 0.3f;
}
