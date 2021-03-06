using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectState : State
{
    Vector2Int index;
    public LevelSelectState(Game _game) : base(_game)
    {

    }

    public override void OnStart()
    {
        game.ObjectsActive(game.levelMenuItems, true);
        game.menuHighlight.SetActive(true);

        index = new Vector2Int(game.levelIndex / (game.levelMenuItems.GetLength(1) - 1), game.levelIndex % (game.levelMenuItems.GetLength(1) - 1));
    }

    public override void UpdateState()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (index.y > 0)
                index.y -= 1;
            else
                index.y = game.levelMenuItems.GetLength(1) - 1;

            game.mainAudioSource.PlayOneShot(game.sounds.menuNavigate);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            if (index.y < game.levelMenuItems.GetLength(1) - 1)
                index.y += 1;
            else
                index.y = 0;

            game.mainAudioSource.PlayOneShot(game.sounds.menuNavigate);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            if (index.x > 0)
                index.x -= 1;
            else
                index.x = game.levelMenuItems.GetLength(0) - 1;

            game.mainAudioSource.PlayOneShot(game.sounds.menuNavigate);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            if (index.x < game.levelMenuItems.GetLength(0) - 1)
                index.x += 1;
            else
                index.x = 0;

            game.mainAudioSource.PlayOneShot(game.sounds.menuNavigate);
        }


        Vector3 oldPosition = game.menuHighlight.transform.position;
        Vector3 newPosition = game.levelMenuItems[index.x, index.y].transform.position;
        game.menuHighlight.transform.position = new Vector3(newPosition.x, newPosition.y, oldPosition.z);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            game.SwitchToState(new MainMenuState(game));
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            game.mainAudioSource.PlayOneShot(game.sounds.menuSelect);
            if (index.y == game.levelMenuItems.GetLength(1) - 1)
            {
                game.SwitchToState(new MainMenuState(game));
                return;
            }
            else
            {
                game.levelIndex = index.x * (game.levelMenuItems.GetLength(1) - 1) + index.y % (game.levelMenuItems.GetLength(1) - 1);
                game.SwitchToState(new GameState(game));
                return;
            }
        }
    }

    public override void OnExit()
    {
        game.ObjectsActive(game.levelMenuItems, false);
        game.menuHighlight.SetActive(false);
    }
}
