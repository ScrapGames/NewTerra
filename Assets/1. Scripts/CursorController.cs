using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.InputSystem;

public class CursorController : MonoBehaviour
{
    public static LayerMask selectionMask;
    public static string selectionTag;
    private const float HOVER_THROTTLE = 0.3f;
    public bool IsObjectUnderCursor { get { return currentHoverObject != null; } }
    private bool IsSelectedUnderCursor { get { return currentHoverObject == currentSelectedObject; } }
    public bool CanInteract { get; set; }

    [SerializeField, Header("UI Links")] public Image cursor = null;
    [SerializeField] private Image hLine = null, vLine = null;

    [Header("Cursor Options"), SerializeField, Range(0, 5)] private float inactivityTime = 1f;
    [SerializeField, Range(0, 1)] private float fadeTime = 0.5f;
    [SerializeField] private Color cursorColor = Color.white;

    [Header("Interaction Options"), SerializeField]
    private Color cursorFadeColor;
    private Vector2 screenMid, lastPos;
    private float lastUpdate;
    private bool isActive;
    private Camera cam;
    private GameObject currentHoverObject = null, currentSelectedObject = null;
    private GameObject plop;
    private Transform currentPlanet;
    private bool isMovingPlop;
    private Vector3 targetPlopPos;
    private Vector3 plopVel;
    private float lastHoverTime;
    private float dist;
    private LayerMask mask;
    private PlayerInput playerInput;
    private float sensitivity;
    private Vector2 screenSize;
    private bool hasInit = false;

    private void Awake()
    {
        screenSize = new Vector2(Screen.width, Screen.height);
        screenMid = screenSize * 0.5f;
    }

    public void Init()
    {
        cam = Camera.main;
        if (hasInit) return;
        cursorFadeColor = cursorColor;
        cursorFadeColor.a = 0;
        cursor.color = hLine.color = vLine.color = cursorFadeColor;
        isActive = false;
        cursor.transform.position = screenMid;
        dist = Vector3.Distance(cam.transform.position, Moon.Target.transform.position);
        playerInput = PlayerController.Instance.playerInput;
        sensitivity = GameManager.Instance.gameSettings.ui_ControllerCursorSensitivity;
        lastPos = screenMid;
        cursor.transform.position = lastPos;
        hLine.transform.position = new Vector2(screenMid.x, lastPos.y);
        vLine.transform.position = new Vector2(lastPos.x, screenMid.y);
        hasInit = true;
    }


    /// <summary>
    /// Moves the cursor to screen position
    /// </summary>
    /// <param name="position">Raw input.</param>
    public void MoveCursor(Vector2 movement)
    {
        // Determine control scheme
        Vector2 position = movement;
        // Keyboard & Mouse
        if (playerInput.currentControlScheme == "Controller")
        {
            // Controller            
            Vector2 cursorDir = movement * Time.deltaTime * (sensitivity * screenSize.x);
            position = lastPos + cursorDir;
            Debug.LogFormat("Movement: {0} - New Position: {1}", movement, position);
        }

        // Clamp position
        position = ScrapGames.Utils.ClampPosition(position, Vector2.zero, screenSize);

        // If still - do nothing
        if (lastPos == position) return;

        if (!isActive)
        {
            SetCursorActivity(true);
        }

        cursor.transform.position = position;
        hLine.transform.position = new Vector2(screenMid.x, position.y);
        vLine.transform.position = new Vector2(position.x, screenMid.y);


        lastUpdate = Time.unscaledTime;
        lastPos = position;
    }

    private void Update()
    {
        if (!hasInit) return;
        InactivityTimer();
        CursorInteract();
        MoveObject();
    }
    private void InactivityTimer()
    {
        if (!isActive || currentHoverObject) return;

        if (Time.unscaledTime > lastUpdate + inactivityTime)
        {
            SetCursorActivity(false);
        }
    }

    public void SetCursorActivity(bool set)
    {
        // Hide cursor
        isActive = set;

        void Fade(Image[] images)
        {
            for (int i = 0; i < images.Length; i++)
            {
                images[i].DOKill();
                images[i].DOFade(set ? 1 : 0, fadeTime);
            }
        }
        Fade(new Image[] { cursor, vLine, hLine });
    }

    private void CursorInteract()
    {
        if (Time.time < lastHoverTime + HOVER_THROTTLE) return;

        if (!isActive)
        {
            // If we are hovering over something, deselect it
            if (IsObjectUnderCursor)
            {
                OnExitHoverObject();
            }
            return;
        }
        if (!CanInteract) return;


        // Raycast from cursor position
        Ray ray = cam.ScreenPointToRay(lastPos);


        RaycastHit[] hits = Physics.RaycastAll(ray, dist, selectionMask);
        if (hits.Length > 0)
        {
            // Loop through till we found our tag
            for (int i = 0; i < hits.Length; i++)
            {
                // If this is not the tag, try the next
                if (!hits[i].collider.CompareTag(selectionTag)) continue;

                // Check if it's still us
                if (hits[i].transform.gameObject == currentHoverObject)
                    return;

                // Set new hover object
                OnEnterHoverObject(hits[i].transform.GetComponent<ICursorInteractable>());
                return;
            }
        }

        // If ray did not hit, or hit was another object
        if (IsObjectUnderCursor)
        {
            // We've left the selected target
            OnExitHoverObject();
        }
    }

    private void OnEnterHoverObject(ICursorInteractable obj)
    {
        if (IsObjectUnderCursor)
            OnExitHoverObject();
        currentHoverObject = (obj as MonoBehaviour).gameObject;
        obj.OnCursorEnter();
        lastHoverTime = Time.time;
    }

    private void OnExitHoverObject()
    {
        if (currentHoverObject == null) return;
        currentHoverObject.GetComponent<ICursorInteractable>().OnCursorExit();
        currentHoverObject = null;
    }

    private void OnSelectObject()
    {
        currentSelectedObject?.GetComponent<ICursorInteractable>().OnDeselect();
        currentSelectedObject = currentHoverObject;
        currentSelectedObject.GetComponent<ICursorInteractable>().OnSelect();
    }

    public void OnDeselectObject()
    {
        currentSelectedObject?.GetComponent<ICursorInteractable>().OnDeselect();
        currentSelectedObject = null;
    }

    public void OnCursorSelectTriggered()
    {
        // Check if active
        if (!isActive)
        {
            // Deselect current selection
            OnDeselectObject();
            return;
        }

        // Check if anything is highlighted
        if (IsObjectUnderCursor)
        {
            // Send select command if it is not already selected
            if (!IsSelectedUnderCursor)
                OnSelectObject();
            return;
        }
        else
        {
            // Nothing selected - send deselect
            OnDeselectObject();
        }
    }


    public void SetMoveObject(PlopObject plopable, Transform currentPlanet)
    {
        this.plop = plopable.gameObject;
        this.currentPlanet = currentPlanet;
        CanInteract = false;
        isMovingPlop = true;

        // Set in mid of screen
        if (Physics.Raycast(cam.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2)),
            out RaycastHit hit, LayerMask.GetMask("Planet")))
        {
            targetPlopPos = hit.point;

            // Get up direction
            Vector3 up = (targetPlopPos - currentPlanet.position).normalized;

            // Position on the planet  
            plop.transform.position = targetPlopPos;

            // Get forward direction        
            Vector3 forward = Vector3.Cross(plop.transform.right, up);

            // Set rotation
            plop.transform.rotation = Quaternion.LookRotation(forward, up);

            plopable.PlopSet += OnBuildingPlop;
        }
    }

    private void OnBuildingPlop(PlopObject plopable)
    {
        plopable.PlopSet -= OnBuildingPlop;

        // Set parent to be the continent
        if (Physics.Raycast(plop.transform.position + plop.transform.up, -plop.transform.up, out RaycastHit plopHit, dist, mask))
        {
            plopable.transform.SetParent(plopHit.transform);
        }

        CanInteract = true;
        isMovingPlop = false;
        plopable = null;
    }

    public bool CheckPlopUnderCursor()
    {
        if (Physics.Raycast(cam.ScreenPointToRay(lastPos), out RaycastHit hit, dist))
        {
            GameObject plop = hit.transform.gameObject;
            if (plop == null) return false;
            if (plop == this.plop) return true;
        }
        return false;
    }

    public void MoveObject()
    {
        if (!isMovingPlop) return;
        if (!isActive) return;

        // Do raycast for planet - if hit send position to move building
        mask = LayerMask.GetMask("Planet"); // TODO: Send mask in

        if (Physics.Raycast(cam.ScreenPointToRay((Vector3)lastPos), out RaycastHit hit, dist, mask))
        {
            targetPlopPos = hit.point;
            Vector3 planetPos = currentPlanet.position;
            float surfaceHeight;

            // Get surface height from plop object
            if (Physics.Raycast(plop.transform.position + (plop.transform.up * 50), -plop.transform.up, out RaycastHit plopHit, dist, mask))
            {
                surfaceHeight = (plopHit.point - planetPos).magnitude;
            }
            else
            {
                surfaceHeight = (hit.point - planetPos).magnitude;
            }

            // Position on the planet  
            Vector3 newPos = Vector3.SmoothDamp(plop.transform.position, targetPlopPos, ref plopVel, 0.23f); // TODO: Set in game vars

            // Get up direction
            Vector3 up = (newPos - planetPos).normalized;

            // Get forward direction        
            Vector3 forward = Vector3.Cross(plop.transform.right, up);

            // Set rotation
            plop.transform.rotation = Quaternion.LookRotation(forward, up);

            // Bump up to surface
            plop.transform.position = planetPos + (
                (newPos - planetPos).normalized * surfaceHeight);
        }
    }

    public void ClearSelection()
    {
        currentSelectedObject = null;
        currentHoverObject = null;
        plop = null;
    }
}

