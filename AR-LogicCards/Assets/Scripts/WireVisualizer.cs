using UnityEngine;

public class WireVisualizer : MonoBehaviour
{
    [SerializeField] private Material wireMaterial;
    [SerializeField] private float wireWidth = 0.01f;

    private void Start()
    {
        if (ConnectionManager.Instance == null)
        {
            Debug.LogError("ConnectionManager.Instance is null! Make sure ConnectionManager is present in the scene.");
            return;
        }
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
        if (wire == null) return;

        GameObject wireObj = new GameObject($"Wire_{wire.sourcePin.parentCard.name}_{wire.targetPin.parentCard.name}");
        wireObj.transform.SetParent(transform);

        LineRenderer lr = wireObj.AddComponent<LineRenderer>();
        lr.startWidth = wireWidth;
        lr.endWidth = wireWidth;
        lr.positionCount = 2;

        if (wireMaterial == null)
            lr.material = new Material(Shader.Find("Sprites/Default"));
        else
            lr.material = wireMaterial;

        lr.startColor = Color.gray;
        lr.endColor = Color.gray;

        wire.visual = lr;
        UpdateWirePosition(wire);
    }

    private void DestroyWireVisual(Wire wire)
    {
        if (wire?.visual != null)
            Destroy(wire.visual.gameObject);
        if (wire != null)
            wire.visual = null;
    }

    private void Update()
    {
        if (ConnectionManager.Instance == null) return;
        foreach (var wire in ConnectionManager.Instance.GetAllWires())
        {
            if (wire?.visual != null)
                UpdateWirePosition(wire);
        }
    }

    private void UpdateWirePosition(Wire wire)
    {
        if (wire.sourcePin == null || wire.targetPin == null) return;
        wire.visual.SetPosition(0, wire.sourcePin.transform.position);
        wire.visual.SetPosition(1, wire.targetPin.transform.position);
    }
}