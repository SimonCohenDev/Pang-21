using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponObject : Collidable
{
    public float maxLength;
    public float speed;

    public Player player { get; set; }

    //debug
    [Header("Debug")]
    [SerializeField] private float length;

    private void Awake()
    {
        Init();
    }
    private void Update()
    {
        updateWeaponObject();
    }

    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        var other = collision.gameObject.GetComponent<Collidable>();
        
        if (other is Ball)
        {
            gameManager.DestroyWeapon(this);
        }
        else if (other is Wall)
        {
            gameManager.DestroyWeapon(this);
        }
        else if (other is Player)
        {
            Physics2D.IgnoreCollision(collision.otherCollider, collision.collider);
        }
        else if (other is Obstacle)
        {
            gameManager.DestroyWeapon(this);
        }
    }


    protected virtual void updateWeaponObject()
    {
        var dTime = Time.deltaTime;
        transform.localScale += Vector3.up * speed * dTime;
        transform.position += Vector3.up * speed / 2 * dTime;
        
        //debug
        length = transform.localScale.y;

        if (length >= maxLength)
        {
            gameManager.DestroyWeapon(this);
        }
    }

    
}
