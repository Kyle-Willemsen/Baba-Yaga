using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightState : BaseState
{


    public override void EnterState(StateManager state)
    {
        
        //state.transform.LookAt(state.player);
    }

    public override void UpdateState(StateManager state)
    {
        Quaternion rotTarget = Quaternion.LookRotation(state.player.position - state.transform.position);
        state.transform.rotation = Quaternion.RotateTowards(state.transform.rotation, rotTarget, state.rotationSpeed * Time.deltaTime);

        Physics.Raycast(state.barrel.position, state.barrel.TransformDirection(Vector3.forward), out state.hit, Mathf.Infinity);
        Debug.DrawRay(state.barrel.position, state.barrel.TransformDirection(Vector3.forward) * state.hit.distance, Color.cyan);
        Debug.Log(state.hit.collider.tag);
        if (state.hit.collider.tag == "Player")
        {
            state.SwitchState(state.ShootState);
        }

        if (state.CanSeePlayer() == false)
        {
            state.SwitchState(state.StaticState);
        }
    }


}
