using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GameController : Singleton<GameController>
{
    public enum GameState
    {
        Pre, Active, Post
    }

    public GameState State = GameState.Pre;

    private void Start()
    {

    }
}
