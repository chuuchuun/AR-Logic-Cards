using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class WireRemovalHandler : MonoBehaviour
{
    private float pressTimer = 0f;
    private Wire pendingWire = null;

    private void OnEnable() => EnhancedTouchSupport.Enable();
    private void OnDisable() => EnhancedTouchSupport.Disable();

    void Update()
    {
        if (Touch.activeTouches.Count > 0)
        {
            Touch touch = Touch.activeTouches[0];
            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
                CheckRaycastAndStartTimer(touch.screenPosition);
            else if (touch.phase == UnityEngine.InputSystem.TouchPhase.Stationary && pendingWire != null)
                UpdateLongPress();
            else if (touch.phase == UnityEngine.InputSystem.TouchPhase.Ended || touch.phase == UnityEngine.InputSystem.TouchPhase.Canceled)
                pendingWire = null;
        }
        else if (Mouse.current != null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
                CheckRaycastAndStartTimer(Mouse.current.position.ReadValue());
            else if (Mouse.current.leftButton.isPressed && pendingWire != null)
                UpdateLongPress();
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
                pendingWire = null;
        }
    }

    private void CheckRaycastAndStartTimer(Vector2 screenPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            // Ignore pins
            if (hit.collider.GetComponent<Pin>() != null) return;

            BoxCollider col = hit.collider as BoxCollider;
            if (col != null)
            {
                foreach (Wire w in ConnectionManager.Instance.GetAllWires())
                    if (w.collider == col) { pendingWire = w; break; }
            }
        }
        pressTimer = 0f;
    }

    private void UpdateLongPress()
    {
        pressTimer += Time.deltaTime;
        if (pressTimer >= 0.5f && pendingWire != null)
        {
            ConnectionManager.Instance.RemoveWire(pendingWire);
            pendingWire = null;
        }
    }
}