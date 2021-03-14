using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameDefaults 
{
    public static readonly Vector2 BallStartVelocity = new Vector2(2f, 2f);
    public static readonly Vector2[] PlayerStartPositions = { new Vector2(0.5f, 1.82f), new Vector2(-0.5f, 1.82f) };

    public static readonly int MaxLives = 3;
}
