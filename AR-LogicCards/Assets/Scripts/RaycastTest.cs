using UnityEngine;

public class RaycastTest : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 10, Color.red, 2f);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
                Debug.Log("Ray hit: " + hit.collider.name);
            else
                Debug.Log("Ray missed");
        }
    }
}
