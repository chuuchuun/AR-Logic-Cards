using UnityEngine;

public class WireRemovalHandler : MonoBehaviour
{
    private float pressTimer = 0f;
    private Wire pendingWire = null;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                BoxCollider col = hit.collider as BoxCollider;
                if (col != null)
                {
                    foreach (Wire w in ConnectionManager.Instance.GetAllWires())
                        if (w.collider == col) { pendingWire = w; break; }
                }
            }
            pressTimer = 0f;
        }
        else if (Input.GetMouseButton(0) && pendingWire != null)
        {
            pressTimer += Time.deltaTime;
            if (pressTimer >= 0.5f)
            {
                ConnectionManager.Instance.RemoveWire(pendingWire);
                pendingWire = null;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            pendingWire = null;
        }
    }
}