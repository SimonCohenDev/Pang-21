using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    private List<Collidable> spawnedObjects;
    private List<GameObjectLifetime> pointBanners;
    private float pointBannerLifetime = 1;

    public void SpawnLevelEntities(LevelData data)
    {
        foreach (var ballData in data.Balls)
        {
            SpawnBalls(ballData);
        }
        foreach (var obstacleData in data.Obstacles)
        {
            SpawnObstacles(obstacleData);
        }
    }
    public void SpawnBalls(BallData balls)
    {
        Ball spawnObject = balls.Prefab;

        for (int i = 0; i < balls.Positions.Length; i++)
        {
            Vector2 spawnPosition = balls.Positions[i];
            var newBall = SpawnEntity(spawnObject, spawnPosition, "Ball");

            newBall.Init(balls);
        }
    }
    public void SpawnObstacles(ObstacleData obstacles)
    {
        Obstacle spawnObject = obstacles.Prefab;
        for (int i = 0; i < obstacles.Positions.Length; i++)
        {
            Vector2 spawnPosition = obstacles.Positions[i];
            var newObstacle = SpawnEntity(spawnObject, spawnPosition, "Obstacle");

            newObstacle.Init(obstacles);
        }
    }
    public TEntity SpawnEntity<TEntity>(TEntity original, Vector2 position, string name) where TEntity : Collidable
    {
        var newEntity = Instantiate(original, position, Quaternion.identity);
        spawnedObjects.Add(newEntity);
        newEntity.name = name + spawnedObjects.OfType<TEntity>().Count();
        return newEntity;
    }
    public void SpawnWeaponObject(Player player)
    {
        //check weapon cooldowns
        if (player.CanFireWeapon())
        {
            var weapon = player.GetWeapon();
            Vector3 origin = weapon.SpawningPoint;
            var weaponObject = Instantiate(weapon.prefab, player.transform.position + origin, Quaternion.identity);
            weaponObject.player = player;
            spawnedObjects.Add(weaponObject);
            player.FireWeapon();
        }
    }

    public TextMeshPro SpawnPointBanner(TextMeshPro original, Vector2 position, int points)
    {
        var tPoints = Instantiate(original, position, Quaternion.identity);
        tPoints.tag = "Score";

        tPoints.text = points.ToString();
        pointBanners.Add(new GameObjectLifetime { GO = tPoints.gameObject, Lifetime = pointBannerLifetime });
        return tPoints;
    }
    public void DestroyAll()
    {
        spawnedObjects.ForEach(x => Destroy(x.gameObject));
        spawnedObjects.Clear();
        pointBanners.ForEach(p => Destroy(p.GO));
        pointBanners.Clear();
    }
    public void KillObject(Collidable collidable)
    {
        spawnedObjects.Remove(collidable);
        Destroy(collidable.gameObject);
    }
    public int GetBallCount()
    {
        return spawnedObjects.OfType<Ball>().Count();
    }

    #region Unity messages
    private void Awake()
    {
        spawnedObjects = new List<Collidable>();
        pointBanners = new List<GameObjectLifetime>();
    }

    private void Update()
    {
        //check for pointBanners and destroy them after lifetime exceeded
        for (int i = 0; i < pointBanners.Count; i++)
        {
            var item = pointBanners[i];
            item.Lifetime -= Time.deltaTime;
            if (item.Lifetime > 0)
            {
                return;
            }
            Destroy(item.GO);
        }
    }

    #endregion
}
