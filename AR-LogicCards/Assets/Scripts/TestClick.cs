using UnityEngine;

public class TestClick : MonoBehaviour
{
    void OnMouseDown()
    {
        Debug.Log("Clicked on " + name);
    }
}