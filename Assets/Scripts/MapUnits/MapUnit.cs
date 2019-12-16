using Mapbox.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Mapbox.Utils;

public class MapUnit : MonoBehaviour
{
    [SerializeField]
    private MapUnitData data;
    [SerializeField]
    private Team team;

    //Positioning Data
    public float rotation;
    public double latitude;
    public double longitude;
    public float altitude;
    public int teamid;

    //Route
    private RouteAlligner routeAlligner;
    public RouteAlligner GetRoute()
    {
        return routeAlligner;
    }
    private bool selected;
    private bool creatingRoute;
    public bool hasRoute;

    //Visualisation
    [SerializeField]
    private Transform rotateObject;
    [SerializeField]
    private Material routeMaterial;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private Color normalColor;
    [SerializeField]
    private Color hoveredColor;
    private List<MeshRenderer> renderers = new List<MeshRenderer>();

    public void Initialize(int teamid, double latitude, double longitude, float altitude, float rotation)
    {
        this.teamid = teamid;
        this.latitude = latitude;
        this.longitude = longitude;
        this.altitude = altitude;
        this.rotation = rotation;
        Rotate(rotation);
    }
    private void Update()
    {
        if (creatingRoute)
        {
            if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                routeAlligner.AddPosition(QuadTreeCameraMovement.Instance.ReturnMousePos());
            }
            else if (Input.GetMouseButtonUp(1))
            {
                //cancel
                routeAlligner.FinishRoute();
                creatingRoute = false;
            }
            routeAlligner.SetPosition(QuadTreeCameraMovement.Instance.followObjectMouse.position);
            hasRoute = true;
        }
        if (hasRoute)
        {
            routeAlligner.UpdateRoute();

            if (QuadTreeCameraMovement.Instance.creatingRoute && !creatingRoute)
                StartCoroutine(CancelRoute());
        }
    }
    IEnumerator CancelRoute()
    {
        yield return new WaitForSeconds(1f);
        QuadTreeCameraMovement.Instance.creatingRoute = false;
    }
    public MapUnitData GetData()
    {
        return data;
    }
    //
    public void Rotate(float angle)
    {
        rotateObject.transform.rotation = Quaternion.Euler(new Vector3(90, 0, -angle));
        rotation = angle;
    }
    public void SetColor(bool hovering)
    {
        if (hovering) spriteRenderer.color = hoveredColor;
        else spriteRenderer.color = normalColor;
    }
    public void ToggleRoute()
    {
        //Toggle route to set all route implementations
        if (hasRoute)
        {
            routeAlligner.ClearRoute();
            routeAlligner = null;
            hasRoute = false;
            ObjectSelector.Instance.UpdateRouteText(true);
        }
        else
        {
            if (!routeAlligner)
            {
                //create new 'RouteAlligner' and set assign latitude and longitude to it 
                routeAlligner = new RouteAlligner(new Vector2d(latitude, longitude), routeMaterial);
            }
            creatingRoute = true;
            QuadTreeCameraMovement.Instance.creatingRoute = true;
        }
    }
    //Mouse downs
    private void OnMouseDown()
    {
        ObjectSelector.Instance.SelectObject(data, this, rotation);
    }
    private void OnMouseOver()
    {
        SetColor(true);
        ObjectSelector.Instance.isHovering = true;
    }
    private void OnMouseExit()
    {
        SetColor(false);
        ObjectSelector.Instance.isHovering = false;
    }
}
