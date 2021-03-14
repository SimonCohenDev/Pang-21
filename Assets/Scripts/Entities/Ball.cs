using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ball : Collidable
{
    private const int SIZE_MULTIPLIER = 5;
    private const float MAX_VELOCITY_y = 10;

    private Rigidbody2D rb;

    private int size;
    private Vector2 ballStartVelocity = GameDefaults.BallStartVelocity;

    [Range(1, 1.05f)]
    public float Bumpiness;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Init();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var other = collision.transform.GetComponent<Collidable>();
        
        if (other is Ball)
        {
            Physics2D.IgnoreCollision(collision.otherCollider, collision.collider);
        }
        else if (other is Player)
        {
            gameManager.PlayerHit(other as Player);
        }
        else if (other is WeaponObject)
        {
            HitByWeapon(other.To<WeaponObject>());
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        var other = collision.transform.GetComponent<Collidable>();
        var initVelocity = GameDefaults.BallStartVelocity;

        if (other is Wall || other is Obstacle)
        {
            var vX = initVelocity.x * Mathf.Sign(rb.velocity.x);
            rb.velocity = new Vector2(vX, Mathf.Clamp(rb.velocity.y, -MAX_VELOCITY_y, MAX_VELOCITY_y));
        }
    }


    public override void Init(EntityData data)
    {
        Init();
        init(data as BallData);
    }
    public void SetVelocity(Vector2 velocity)
    {
        rb.velocity = velocity;
    }
    public int GetSize()
    {
        return size;
    }
    public void SetSize(int size)
    {
        this.size = size;
        transform.localScale = Vector3.one * size * SIZE_MULTIPLIER;
    }
    public void HitByWeapon(WeaponObject weapon)
    {
        if (size == 1)
        {
            //display hit points
            //play a sound
            //play animation
            gameManager.KillBall(this);
        }
        else
        {
            SetSize(size - 1);

            var clone = gameManager.SplitBall(this, rb.velocity); 
        }

        gameManager.ScorePlayer(weapon.player, transform.position);

    }

    private void init(BallData ballData)
    {
        SetSize(ballData.InitialSize);
        SetVelocity(GameDefaults.BallStartVelocity);
    }
}
