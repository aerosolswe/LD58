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

    public float RoundTime = 60.0f;
    public float CurrentTime = 0.0f;

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
        CurrentTime = 0;
        State = GameState.Active;
        UI.Instance.ActiveScreen.Show();
    }

    public void SetPostState()
    {
        State = GameState.Post;
        //UI.Instance.ActiveScreen.Show();
    }

    private void Update()
    {
        if (State == GameState.Active)
        {
            CurrentTime += Time.deltaTime;

            float time = Mathf.Max(RoundTime - CurrentTime, 0);
            UI.Instance.ActiveScreen.TimeText.text = (int)time + " sec";

            if (CurrentTime > RoundTime)
            {
                CurrentTime = 0;
                SetPostState();
            }
        }
    }
}
