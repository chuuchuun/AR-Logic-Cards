using UnityEngine;
using UnityEngine.EventSystems;

public class GestureInputHandler : MonoBehaviour
{
    private Pin _selectedOutputPin = null;

    void Update()
    {
        // Для редактора – мышь, для телефона – касания
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
                HandleTap(touch.position);
        }
        else if (Input.GetMouseButtonDown(0))
        {
            HandleTap(Input.mousePosition);
        }
    }

    private void HandleTap(Vector2 screenPos)
    {
        // Игнорируем тап по UI
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f))
        {
            Pin pin = hit.collider.GetComponent<Pin>();
            if (pin == null) return;

            if (pin.pinType == PinType.Output)
            {
                // Выбираем выход
                _selectedOutputPin = pin;
                HighlightAllPins(false);
                pin.SetSelected(true);
                Debug.Log($"Выбран выход: {pin.parentCard.name}");
            }
            else if (pin.pinType == PinType.Input)
            {
                // Если есть выбранный выход – создаём провод
                if (_selectedOutputPin != null)
                {
                    ConnectionManager.Instance.AddWire(_selectedOutputPin, pin);
                    _selectedOutputPin.SetSelected(false);
                    _selectedOutputPin = null;
                }
                else
                {
                    Debug.Log("Сначала выберите выход (Output)");
                }
            }
        }
        else
        {
            // Тап по пустому месту – сбрасываем выбор
            if (_selectedOutputPin != null)
            {
                _selectedOutputPin.SetSelected(false);
                _selectedOutputPin = null;
                Debug.Log("Выбор отменён");
            }
        }
    }

    private void HighlightAllPins(bool highlight)
    {
        // Можно реализовать подсветку всех пинов, если нужно
    }
}