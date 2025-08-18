using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public List<Node> neighbors = new List<Node>();
    private MeshRenderer meshRenderer;
    public bool enableExtraLayer = false;
    public bool changeScene = false;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void Highlight(bool state)
    {
        if (meshRenderer != null)
        {
            meshRenderer.enabled = state;
        }
    }

    private void OnMouseEnter()
    {
        Highlight(true);
    }

    private void OnMouseExit()
    {
        Highlight(false);
    }
}