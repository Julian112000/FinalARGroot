using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Utils;
using Mapbox.Unity.Map;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> objects = new List<GameObject>();

    private Vector2d[] locations;

    public void PlaceObject(int type, Vector2d location)
    {

    }
}
