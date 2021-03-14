using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Collidable
{
    public float MoveSpeed;

    private Rigidbody2D rb;
    private Vector2 movement;
    private Weapon currentWeapon;
    private float weaponCooldown;

    public override void Init(EntityData data)
    {
        MoveSpeed = GameManager.Instance.PlayerInitialSpeed;
    }
    public void Move(Vector2 input)
    {
        movement = input;
    }
    public Weapon GetWeapon()
    {
        return currentWeapon;
    }
    public void SetWeapon(Weapon weapon)
    {
        currentWeapon = weapon;
        weaponCooldown = weapon.FireCooldown;
    }
    public bool CanFireWeapon()
    {
        return weaponCooldown <= 0;
    }
    public void FireWeapon()
    {
        weaponCooldown = currentWeapon.FireCooldown;
    }
    public void UpdateCooldownTimer()
    {
        weaponCooldown -= Time.deltaTime;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Init();
    }
    private void FixedUpdate()
    {
        rb.velocity = movement * MoveSpeed;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        var other = collision.transform.GetComponent<Collidable>();
        if (other is Player || other is WeaponObject)
        {
            Physics2D.IgnoreCollision(collision.otherCollider, collision.collider);
        }
    }

    
}
