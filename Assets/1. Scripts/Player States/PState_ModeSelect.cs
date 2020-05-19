using UnityEngine;
using UnityEngine.InputSystem;

public class PState_ModeSelect : PState_UI
{

    private State<PlayerController> previousState;
    public PState_ModeSelect(State<PlayerController> currentState)
    {
        previousState = currentState;
    }
    private UI_ModeSelection uiModeSelection;

    private InputAction action_Move;

    public override void OnDisable()
    {
        base.OnDisable();
        UIManager.Instance.UI_ModeSelection.Show(false);
        action_Move.performed -= OnMove;

        // Unsubscribe from button events
        uiModeSelection.ClearButtonListeners();

        GameManager.Instance.cameraJuice.ClearBlur();
    }

    public override void OnEnable(PlayerController owner, StateMachine<PlayerController> newStateMachine)
    {
        showCursor = false;

        base.OnEnable(owner, newStateMachine);

        action_Move = map.FindAction("Move");
        action_Move.performed += OnMove;

        // Show mode UI
        uiModeSelection = UIManager.Instance.UI_ModeSelection;
        uiModeSelection.Show(true);

        // Subscribe to button events
        uiModeSelection.button_Orbit.onClick.AddListener(() => ownerStateMachine.CurrentState = new PState_OrbitCam());
        uiModeSelection.button_Build.onClick.AddListener(() => ownerStateMachine.CurrentState = new PState_BuildMenu());
        uiModeSelection.button_Messages.onClick.AddListener(() => ownerStateMachine.CurrentState = new PState_NotificationView());
        uiModeSelection.button_ResourceView.onClick.AddListener(() => ownerStateMachine.CurrentState = new PState_ResourceView());

        // Blur Background
        GameManager.Instance.cameraJuice.Blur();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        if (!GameManager.Instance.IsUsingController) return;

        Vector2 dir = context.ReadValue<Vector2>();
        if (dir == Vector2.up)
            uiModeSelection.button_Messages.Select();
        else if (dir == Vector2.down)
            uiModeSelection.button_Build.Select();
        else if (dir == Vector2.left)
            uiModeSelection.button_Build.Select();
        else
            uiModeSelection.button_Orbit.Select();

    }
}