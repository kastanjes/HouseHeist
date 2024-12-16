using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class SortingLayerSetter : MonoBehaviour
{
    public string sortingLayerName = "Default"; // Default sorting layer
    public int sortingOrder = 0;               // Order in the layer

    void Start()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.sortingLayerName = sortingLayerName;
            renderer.sortingOrder = sortingOrder;
        }
    }
}
