using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuState : State
{
    int index;
    public MainMenuState(Game _game) : base(_game)
    {

    }

    public override void OnStart()
    {
        game.ObjectsActive(game.mainMenuItems, true);
        game.menuHighlight.SetActive(true);

        index = 0;
    }

    public override void UpdateState()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (index > 0)
                index -= 1;
            else
                index = game.mainMenuItems.Length - 1;

            game.mainAudioSource.PlayOneShot(game.sounds.menuNavigate);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            if (index < game.mainMenuItems.Length - 1)
                index += 1;
            else
                index = 0;

            game.mainAudioSource.PlayOneShot(game.sounds.menuNavigate);
        }

        Vector3 oldPosition = game.menuHighlight.transform.position;
        Vector3 newPosition = game.mainMenuItems[index].transform.position;
        game.menuHighlight.transform.position = new Vector3(newPosition.x, newPosition.y, oldPosition.z);

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            if (index == 0)
            {
                game.levelIndex = 0;
                game.mainAudioSource.PlayOneShot(game.sounds.menuSelect);
                game.SwitchToState(new LevelSelectState(game));
                return;
            }
            else if (index == 1)
            {
                Application.Quit();
            }
        }
    }

    public override void OnExit()
    {
        game.ObjectsActive(game.mainMenuItems, false);
        game.menuHighlight.SetActive(false);
    }
}
