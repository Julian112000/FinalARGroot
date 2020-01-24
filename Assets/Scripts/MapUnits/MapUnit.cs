using Mapbox.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Mapbox.Utils;

public class MapUnit : MonoBehaviour
{
    //Data of the map unit [contains id, name, type etc.]
    [SerializeField]
    private MapUnitData data;
    //enum of the team variable
    [SerializeField]
    private Team team;

    //Orientation Data
    public float rotation;      //rotation
    public double latitude;     //latitude
    public double longitude;    //longitude
    public float altitude;      //atltitude
    public int teamid;          //teamid

    //Route
    private RouteAlligner routeAlligner;    //main routealligner component
    public RouteAlligner GetRoute()
    {
        return routeAlligner;
    }      //returns the routealligner in public               
    private bool creatingRoute;             //bool to check if unit is creating route
    public bool hasRoute;                   //bool toch check if unit has a route

    //Visualisation
    //parent of the object that rotates when player uses the slider
    [SerializeField]
    private Transform rotateObject;
    //Routematerial for the route component
    [SerializeField]
    private Material routeMaterial;
    //spriterenderer for the color of the unit's icon
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    //normal color when not selected
    [SerializeField]
    private Color normalColor;
    //color when hovering over object
    [SerializeField]
    private Color hoveredColor;
    //list of all the meshrenderers inside an object
    private List<MeshRenderer> renderers = new List<MeshRenderer>();

    //Void to initialize the unit
    public void Initialize(int teamid, double latitude, double longitude, float altitude, float rotation)
    {
        this.teamid = teamid;       //set teamid [0 = allie, 1 = opfor]
        this.latitude = latitude;   //set latitude
        this.longitude = longitude; //set longitude
        this.altitude = altitude;   //set altitude [height]
        this.rotation = rotation;   //set rotation
        //Rotation object with new given rotation
        Rotate(rotation);           
    }
    private void Update()
    {
        //when object is creating route
        if (creatingRoute)
        {
            //when the mouse is released and is not pointing over an other object
            if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                //add a new position to the routalligner component
                routeAlligner.AddPosition(QuadTreeCameraMovement.Instance.ReturnMousePos());
            }
            //when the player right clicks, finish the route and autocomplete it
            else if (Input.GetMouseButtonUp(1))
            {
                //finish
                routeAlligner.FinishRoute();
                creatingRoute = false;
            }
            //let the routealligner follow the mouse
            routeAlligner.SetPosition(QuadTreeCameraMovement.Instance.followObjectMouse.position);
            hasRoute = true;
        }
        //when unit already has a route
        if (hasRoute)
        {
            //update that route constant with the new zoom of the map and position of the origin
            routeAlligner.UpdateRoute();
            //check if someone is creating a route
            if (QuadTreeCameraMovement.Instance.creatingRoute && !creatingRoute)
                StartCoroutine(CancelRoute());
        }
    }
    //IEnumerator to cancel the route
    IEnumerator CancelRoute()
    {
        yield return new WaitForSeconds(1f);
        //cancel the route
        QuadTreeCameraMovement.Instance.creatingRoute = false;
    }
    //Return data
    public MapUnitData GetData()
    {
        return data;
    }
    //Main rotate function to set new rotation of the object
    public void Rotate(float angle)
    {
        rotateObject.transform.rotation = Quaternion.Euler(new Vector3(90, 0, -angle));
        rotation = angle;
    }
    //Main function to change the color of the object
    public void SetColor(bool hovering)
    {
        if (hovering) spriteRenderer.color = hoveredColor;  //Change sprite color to hovered color
        else spriteRenderer.color = normalColor;            //Change sprite color to normal color
    }
    //Toggleroute main function
    public void ToggleRoute()
    {
        //When unit already have a route
        if (hasRoute)
        {
            //destroy the route and set new text to 'create route'
            routeAlligner.ClearRoute();
            routeAlligner = null;
            hasRoute = false;
            ObjectSelector.Instance.UpdateRouteText(true);
        }
        //otherwise
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
        //select object with all given data
        ObjectSelector.Instance.SelectObject(data, this, rotation);
    }
    private void OnMouseOver()
    {
        //set color when hovering
        SetColor(true);
        ObjectSelector.Instance.isHovering = true;
    }
    private void OnMouseExit()
    {
        //set color when not hovering anymore
        SetColor(false);
        ObjectSelector.Instance.isHovering = false;
    }
}
