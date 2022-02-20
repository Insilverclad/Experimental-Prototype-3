using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartState : State
{
    float cameraSizeStart;
    float cameraSizeGame;

    float transitionTimer;
    float transitionDuration;
    bool transition;

    public StartState(Game _game) : base(_game)
    {

    }

    public override void OnStart()
    {
        cameraSizeStart = 25.0f;
        cameraSizeGame = 10.0f;

        transitionTimer = 0f;
        transitionDuration = 1.0f;
        transition = false;
    }

    public override void UpdateState()
    {
        if (!transition)
        {
            game.startText.alpha = (Mathf.Sin(Time.realtimeSinceStartup * 4.0f) + 1.0f) / 2.0f;

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                transition = true;
                game.startText.gameObject.SetActive(false);
            }
        }
        else
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
}
