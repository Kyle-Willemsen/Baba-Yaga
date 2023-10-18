using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticState : BaseState
{
    public override void EnterState(StateManager state)
    {
        state.anim.SetBool("isSearching", true);
    }

    public override void UpdateState(StateManager state)
    {

    }
}
