using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionInteraction : MonoBehaviour
{
    public string regionName;

    private void OnMouseDown()
    {
        Debug.Log($"Region clicked: {regionName}");
        // Add code for selecting/deselecting the region.
    }
}
