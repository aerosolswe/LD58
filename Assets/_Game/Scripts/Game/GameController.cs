using GCG;
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
        UserDataManager.Init();
        SetPreState();
    }

    public void SetPreState()
    {
        State = GameState.Pre;
        UI.Instance.PreScreen.Show();
    }

    public void SetActiveState()
    {
        State = GameState.Active;
        UI.Instance.ActiveScreen.Show();
    }
}
