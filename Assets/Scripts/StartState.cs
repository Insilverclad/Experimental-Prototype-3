using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartState : State
{
    float cameraSizeStart;
    float cameraSizeGame;

    float transitionTimer;
    float transitionDuration;

    public StartState(Game _game) : base(_game)
    {

    }

    public override void OnStart()
    {
        cameraSizeStart = 25.0f;
        cameraSizeGame = 10.0f;

        transitionTimer = 0f;
        transitionDuration = 5.0f;
    }

    public override void UpdateState()
    {
        transitionTimer += Time.deltaTime;

        float t = transitionTimer / transitionDuration;
        t = t * t * (3f - 2f * t);
        game.cam.orthographicSize = Mathf.Lerp(cameraSizeStart, cameraSizeGame, t);

        if (transitionTimer >= transitionDuration)
        {
            game.cam.orthographicSize = cameraSizeGame;

            game.SwitchToState(new MainMenuState(game));
            return;
        }
    }
}
