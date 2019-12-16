using Mapbox.Examples;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouteAlligner : MonoBehaviour
{
    //position used to get waypoint vector2d's
    private List<Vector2d> positions = new List<Vector2d>();
    public List<Vector2d> GetPositions()
    {
        return positions;
    }
    //component linerenderer
    private LineRenderer lineRenderer;

    //constructor that will be called when route is made
    public RouteAlligner(Vector2d position, Material material)
    {
        //create new local lineobject and assign with data
        GameObject lineObject = new GameObject();
        lineRenderer = lineObject.AddComponent<LineRenderer>();
        lineRenderer.sharedMaterial = material;
        lineRenderer.widthMultiplier = 0.5f;
        lineObject.layer = 8;

        positions.Add(position);
    }
    //set position to linerenderers
    public void SetPosition(Vector3 pos)
    {
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, pos);
    }
    //Add position to linerenderer
    public void AddPosition(Vector2d position)
    {
        lineRenderer.positionCount = lineRenderer.positionCount + 1;
        positions.Add(position);
    }
    //Finish route, add last position to last knew position
    public void FinishRoute()
    {
        SpawnOnMap.Instance.SetRouteAmount(1);
        positions.Add(positions[positions.Count - 1]);
    }
    //Update route with GeoToWorldPosition, so the scale is right
    public void UpdateRoute()
    {
        for (int i = 0; i < positions.Count; i++)
        {
            lineRenderer.SetPosition(i, AbstractMap.Instance.GeoToWorldPosition(positions[i], true));
        }
    }
    //Clear the route when route is deleted for some reason
    public void ClearRoute()
    {
        //reset route amount in 'SpawnOnMap' script
        SpawnOnMap.Instance.SetRouteAmount(-1);
        lineRenderer.positionCount = 0;
        positions.Clear();
        Destroy(this);
    }
}
