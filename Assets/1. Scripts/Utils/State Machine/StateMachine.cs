
public class StateMachine<T>
{
    T ownerObject;
    State<T> currentState;

    public State<T> CurrentState
    {
        get { return currentState; }
        set {
            if (currentState != null)
            {
                currentState.OnDisable();
            }

            currentState = value;

            if (currentState != null)
                currentState.OnEnable(ownerObject, this);
        }
    }

    public StateMachine(State<T> defaultState, T owner)
    {
        ownerObject = owner;
        CurrentState = defaultState;
    }

    public void Update() // Call this during normal update loop
    {
        if (currentState != null)
        {
            currentState.CheckForNewState();
            currentState.Update();
        }
    }

    public void FixedUpdate()// Call this during normal fixed update loop
    {
        if (currentState != null)
        {
            currentState.FixedUpdate();
        }
    }
}
