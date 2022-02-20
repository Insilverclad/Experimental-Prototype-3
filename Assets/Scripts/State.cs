using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    protected Game game;
    public State(Game _game)
    {
        game = _game;
    }

    public virtual void UpdateState()
    {

    }

    public virtual void OnStart()
    {

    }

    public virtual void OnExit()
    {

    }
}
