using System.Collections.Generic;
using UnityEngine;

public class Column : MonoBehaviour
{
    public List<ColumnDestractable> _parts = new List<ColumnDestractable>();

    private void Awake()
    {
        foreach (var part in _parts)
        {
            part.onDestroyed += DisableHighParts;
        }
    }

    private void DisableHighParts(ColumnDestractable origin)
    {
        foreach (var part in _parts)
        {
            if (part.transform.position.y >= origin.transform.position.y)
            {
                part.onDestroyed -= DisableHighParts;
                part.Destroy();
            }
        }
    }
}