using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectState : State
{
    int index;
    public LevelSelectState(Game _game) : base(_game)
    {

    }

    public override void OnStart()
    {
        game.ObjectsActive(game.levelMenuItems, true);
        game.menuHighlight.SetActive(true);

        index = game.levelIndex;
    }

    public override void UpdateState()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (index > 0)
                index -= 1;
            else
                index = game.levelMenuItems.Length - 1;

            game.audioSource.PlayOneShot(game.sounds.menuSelect);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            if (index < game.levelMenuItems.Length - 1)
                index += 1;
            else
                index = 0;

            game.audioSource.PlayOneShot(game.sounds.menuSelect);
        }
            

        Vector3 oldPosition = game.menuHighlight.transform.position;
        Vector3 newPosition = game.levelMenuItems[index].transform.position;
        game.menuHighlight.transform.position = new Vector3(newPosition.x, newPosition.y, oldPosition.z);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            game.SwitchToState(new MainMenuState(game));
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            if (index == game.levelMenuItems.Length - 1)
            {
                game.SwitchToState(new MainMenuState(game));
                return;
            }
            else
            {
                game.levelIndex = index;
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
