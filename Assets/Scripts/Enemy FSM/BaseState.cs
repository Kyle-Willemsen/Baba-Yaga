using UnityEngine;

public abstract class BaseState 
{
    public abstract void EnterState(StateManager stateManager);

    public abstract void UpdateState(StateManager stateManager);
}
