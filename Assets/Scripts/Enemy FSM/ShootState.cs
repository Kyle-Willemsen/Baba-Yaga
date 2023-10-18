using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootState : BaseState
{
    public RaycastHit hit;
    float damage = 1;

    public override void EnterState(StateManager state)
    {
        Shoot(state);
    }

    public override void UpdateState(StateManager state)
    {

    }

    void Shoot(StateManager state)
    {
        //Debug.Log("Shoot");

        //state.SwitchState(state.SightState);
        state.StartCoroutine(state.Shoot());

    }
}
