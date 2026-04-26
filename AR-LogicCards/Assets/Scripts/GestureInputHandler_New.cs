using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class GestureInputHandler_New : MonoBehaviour
{
    private Pin _selectedOutputPin = null;

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    private void Update()
    {
        if (Touch.activeTouches.Count > 0)
        {
            Touch touch = Touch.activeTouches[0];
            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
                HandleTap(touch.screenPosition);
        }
        else if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleTap(Mouse.current.position.ReadValue());
        }
    }

    private void HandleTap(Vector2 screenPos)
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            Pin pin = hit.collider.GetComponent<Pin>();
            if (pin == null) return;

            // --- NEW: Check if the card is allowed in the current level ---
            if (LevelManager.Instance != null && !LevelManager.Instance.IsCardAllowed(pin.parentCard.name))
            {
                string allowedList = LevelManager.Instance.GetAllowedCardsList();
                UIManager.Instance?.ShowInvalidCardsPopup(
                    $"Card '{pin.parentCard.name}' is not allowed in this level.\n\nAllowed cards for this level:\n{allowedList}"
                );
                return; // Block further interaction with this card
            }
            // ---------------------------------------------------------------

            if (pin.pinType == PinType.Output)
            {
                _selectedOutputPin = pin;
                ClearAllHighlights();
                pin.SetSelected(true);
                Debug.Log($"Selected output on {pin.parentCard.name}");
            }
            else if (pin.pinType == PinType.Input)
            {
                if (_selectedOutputPin != null)
                {
                    ConnectionManager.Instance.AddWire(_selectedOutputPin, pin);
                    _selectedOutputPin.SetSelected(false);
                    _selectedOutputPin = null;
                }
                else
                {
                    Debug.Log("Select an output first");
                }
            }
        }
        else
        {
            if (_selectedOutputPin != null)
            {
                _selectedOutputPin.SetSelected(false);
                _selectedOutputPin = null;
                Debug.Log("Selection cancelled");
            }
        }
    }

    private void ClearAllHighlights()
    {
        // Optional: implement clearing visual highlights if you have multiple pins
    }
}