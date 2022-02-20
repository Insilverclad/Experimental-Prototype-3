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
        game.mainMenu.gameObject.SetActive(true);
        game.selectionBox.SetActive(true);

        index = 0;
    }

    public override void UpdateState()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (index > 0)
                index -= 1;
            else
                index = game.mainMenu.buttons.Length - 1;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            if (index < game.mainMenu.buttons.Length - 1)
                index += 1;
            else
                index = 0;
        }

        Vector3 oldPosition = game.selectionBox.transform.position;
        Vector3 newPosition = game.mainMenu.buttons[index].transform.position;
        game.selectionBox.transform.position = new Vector3(newPosition.x, newPosition.y, oldPosition.z);

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            if (index == 0)
            {
                game.levelIndex = 0;
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
        game.mainMenu.gameObject.SetActive(false);
        game.selectionBox.SetActive(false);
    }
}
