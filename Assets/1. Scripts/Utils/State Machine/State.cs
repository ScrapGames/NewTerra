
public abstract class State<T>
{
    protected T ownerObject; // This will be the reference to the object the state is running on
    protected StateMachine<T> ownerStateMachine;    // This will be the reference to the owners state machine
    const float THINK_TICK_TIME = 0.1f;
    float thinkTickTimer;
    protected int CurrentThinkTick { get; private set; }

    public abstract void CheckForNewState();
    
    public virtual void Update()
    {
        if (thinkTickTimer >= THINK_TICK_TIME)
        {
            CurrentThinkTick++;
            thinkTickTimer = 0;
            ThinkTick();
        }
        else
            thinkTickTimer += UnityEngine.Time.deltaTime;
    }

    public abstract void ThinkTick();

    public virtual void OnEnable(T owner, StateMachine<T> newStateMachine)
    {
        ownerObject = owner;
        ownerStateMachine = newStateMachine;
    }

    protected int GetNextThinkTick(int min, int max)
    {
        return CurrentThinkTick + UnityEngine.Random.Range(min, max);
    }

    public virtual void OnDisable() { }
    public virtual void FixedUpdate() { }
}
