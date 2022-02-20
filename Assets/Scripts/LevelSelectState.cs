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
        game.levelMenu.gameObject.SetActive(true);
        game.selectionBox.SetActive(true);

        index = game.levelIndex;
    }

    public override void UpdateState()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (index > 0)
                index -= 1;
            else
                index = game.levelMenu.buttons.Length - 1;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            if (index < game.levelMenu.buttons.Length - 1)
                index += 1;
            else
                index = 0;
        }
            

        Vector3 oldPosition = game.selectionBox.transform.position;
        Vector3 newPosition = game.levelMenu.buttons[index].transform.position;
        game.selectionBox.transform.position = new Vector3(newPosition.x, newPosition.y, oldPosition.z);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            game.SwitchToState(new MainMenuState(game));
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            if (index == game.levelMenu.buttons.Length - 1)
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
        game.levelMenu.gameObject.SetActive(false);
        game.selectionBox.SetActive(false);
    }
}
