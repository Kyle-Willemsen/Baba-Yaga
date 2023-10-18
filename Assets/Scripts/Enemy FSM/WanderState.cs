using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderState : BaseState
{
    public override void EnterState(StateManager state)
    {
        FindWaypoint(state);
    }

    public override void UpdateState(StateManager state)
    {


    }

    public void FindWaypoint(StateManager state)
    {
        Vector3[] waypoints = new Vector3[state.pathHolder.childCount];
        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = state.pathHolder.GetChild(i).position;
            waypoints[i] = new Vector3(waypoints[i].x, state.transform.position.y, waypoints[i].z);
        }
        state.StartCoroutine(state.FollowPath(waypoints));
    }

}
