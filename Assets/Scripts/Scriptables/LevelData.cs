using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Level0", menuName = "Pang/Level")]
public class LevelData : ScriptableObject
{
    public Sprite Background;
    public BallData[] Balls;
    public ObstacleData[] Obstacles;
}


public abstract class EntityData { }

[System.Serializable]
public class EntityData<TEntity> : EntityData where TEntity : Collidable
{
    public TEntity Prefab;
    
    public Vector2[] Positions;
}

[System.Serializable]
public class BallData : EntityData<Ball>
{
    public int InitialSize = 4;
}

[System.Serializable]
public class ObstacleData : EntityData<Obstacle>
{
    public bool Destructable;
    public int Health = 3;
}





