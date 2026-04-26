using UnityEngine;

public class WireVisualizer : MonoBehaviour
{
    [SerializeField] private float wireWidth = 0.01f;

    private void Start()
    {
        ConnectionManager.Instance.OnWireAdded += CreateWireVisual;
        ConnectionManager.Instance.OnWireRemoved += DestroyWireVisual;
    }

    private void OnDestroy()
    {
        if (ConnectionManager.Instance != null)
        {
            ConnectionManager.Instance.OnWireAdded -= CreateWireVisual;
            ConnectionManager.Instance.OnWireRemoved -= DestroyWireVisual;
        }
    }

    private void CreateWireVisual(Wire wire)
    {
        GameObject wireObj = new GameObject($"Wire_{wire.sourcePin.parentCard.name}_{wire.targetPin.parentCard.name}");
        wireObj.transform.SetParent(transform);

        LineRenderer lr = wireObj.AddComponent<LineRenderer>();
        lr.startWidth = wireWidth;
        lr.endWidth = wireWidth;
        lr.positionCount = 2;

        Material mat = new Material(Shader.Find("Sprites/Default"));
        if (mat == null) mat = new Material(Shader.Find("Unlit/Color"));
        mat.color = Color.gray;
        lr.material = mat;

        BoxCollider coll = wireObj.AddComponent<BoxCollider>();
        coll.isTrigger = true;

        wire.visual = lr;
        wire.collider = coll;

        UpdateWireVisual(wire);
    }

    private void DestroyWireVisual(Wire wire)
    {
        if (wire.visual != null)
            Destroy(wire.visual.gameObject);
    }

    private void Update()
    {
        if (ConnectionManager.Instance == null) return;
        foreach (Wire wire in ConnectionManager.Instance.GetAllWires())
            if (wire.visual != null)
                UpdateWireVisual(wire);
    }

    private void UpdateWireVisual(Wire wire)
    {
        if (wire.sourcePin == null || wire.targetPin == null) return;

        Vector3 start = wire.sourcePin.transform.position;
        Vector3 end = wire.targetPin.transform.position;

        wire.visual.SetPosition(0, start);
        wire.visual.SetPosition(1, end);

        if (wire.collider != null)
        {
            Vector3 dir = end - start;
            float length = dir.magnitude;
            wire.collider.transform.position = start + dir * 0.5f;
            wire.collider.transform.rotation = Quaternion.LookRotation(dir);
            wire.collider.size = new Vector3(wireWidth, wireWidth, length);
        }

        bool value = wire.sourcePin.parentCard.currentValue;
        Color targetColor = value ? Color.green : Color.gray;

        if (wire.visual.material != null)
            wire.visual.material.color = targetColor;

        wire.visual.startColor = targetColor;
        wire.visual.endColor = targetColor;
    }
}