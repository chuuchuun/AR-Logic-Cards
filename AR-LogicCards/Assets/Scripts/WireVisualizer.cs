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
        GameObject wireObj = new GameObject($"Wire_{wire.sourcePin.parentCard.name}_{wire.targetPin.parentCard.name}");
        wireObj.transform.SetParent(transform);

        LineRenderer lr = wireObj.AddComponent<LineRenderer>();
        lr.startWidth = wireWidth;
        lr.endWidth = wireWidth;
        lr.positionCount = 2;
        lr.material = wireMaterial ?? new Material(Shader.Find("Sprites/Default"));
        lr.startColor = Color.gray;
        lr.endColor = Color.gray;

        BoxCollider collider = wireObj.AddComponent<BoxCollider>();
        collider.isTrigger = true; 

        wire.visual = lr;
        wire.collider = collider;   

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

        Vector3 start = wire.sourcePin.transform.position;
        Vector3 end = wire.targetPin.transform.position;
        wire.visual.SetPosition(0, start);
        wire.visual.SetPosition(1, end);

        if (wire.collider != null)
        {
            Vector3 dir = end - start;
            float length = dir.magnitude;
            wire.collider.transform.position = start + dir / 2;
            wire.collider.transform.rotation = Quaternion.LookRotation(dir);
            wire.collider.size = new Vector3(wireWidth, wireWidth, length);
        }
    }
}