using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Obstacle : Collidable
{
    public bool Destructable;
    public int Health = 2;

    public override void Init(EntityData data)
    {
        init(data as ObstacleData);
    }

    private void init(ObstacleData data)
    {
        Destructable = data.Destructable;
        Health = data.Health;
    }
    private void HitByWeapon(WeaponObject weaponObject)
    {
        Health -= 1;
        if (Health == 0)
        {
            gameManager.DestroyObstacle(this);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var other = collision.gameObject.GetComponent<Collidable>();
        Debug.Log("Weapon hit " + other.name);
        if (other is WeaponObject)
        {
            HitByWeapon(other.To<WeaponObject>());
        }
    }

}
