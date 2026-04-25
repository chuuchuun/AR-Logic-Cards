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
        // Включаем EnhancedTouch для поддержки множественных касаний
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    private void Update()
    {
        // Для телефона: обрабатываем первый активный тап
        if (Touch.activeTouches.Count > 0)
        {
            Touch touch = Touch.activeTouches[0];
            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                HandleTap(touch.screenPosition);
            }
        }
        // Для редактора: обрабатываем клик мыши
        else if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleTap(Mouse.current.position.ReadValue());
        }
    }

    private void HandleTap(Vector2 screenPos)
    {
        Debug.Log("HandleTap called at " + screenPos);
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("Tap over UI, ignoring");
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            Debug.Log("Raycast hit: " + hit.collider.name);
            Pin pin = hit.collider.GetComponent<Pin>();
            if (pin == null)
            {
                Debug.Log("No Pin component on hit object");
                return;
            }
            Debug.Log("Pin found, type: " + pin.pinType);

            if (pin.pinType == PinType.Output)
            {
                Debug.Log("Output pin selected");
                _selectedOutputPin = pin;
                ClearAllHighlights();
                pin.SetSelected(true);
            }
            else if (pin.pinType == PinType.Input)
            {
                Debug.Log("Input pin tapped");
                if (_selectedOutputPin != null)
                {
                    ConnectionManager.Instance.AddWire(_selectedOutputPin, pin);
                    _selectedOutputPin.SetSelected(false);
                    _selectedOutputPin = null;
                }
                else
                    Debug.Log("No output selected");
            }
        }
        else
        {
            Debug.Log("Raycast missed");
            if (_selectedOutputPin != null)
            {
                _selectedOutputPin.SetSelected(false);
                _selectedOutputPin = null;
            }
        }
    }

    private void ClearAllHighlights()
    {
        // Можно реализовать, если нужно снять выделение со всех пинов
        // Например, найти все Pin и вызвать SetSelected(false)
    }
}