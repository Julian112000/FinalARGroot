using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ObjectSelector : MonoBehaviour
{
    //Instance of ObjectSelector when called to select objects
    public static ObjectSelector Instance;

    //current selected object
    public MapUnit selectedObject;
    //called when mouse is hovering over object
    public bool isHovering;

    //Visualisation
    [SerializeField]
    private Text nametext;          //Text component of current selected unit's name
    [SerializeField]
    private Text idtext;            //Text component of current selected unit's ID
    [SerializeField]
    private Image iconimage;        //Image component of current selected unit's icon
    [SerializeField]
    private Slider rotationSlider;  //Slider component for rotation

    //Route
    [SerializeField]
    private string[] routenames;    //string component either 'delete' or 'create' route
    [SerializeField]
    private Text routeText;         //Text component of route either 'delete' or 'create'

    private void Awake()
    {
        //set instance to this script when game started
        Instance = this;
        gameObject.SetActive(false);
    }
    public void SelectModel(MapUnitDataToText data)
    {
        //update UI with selected model
        CurrentSelectedModel.Instance.UpdateUI(data);
    }
    private void Update()
    {
        //when clicked on map and outside of units, deactivate objectselector ui
        if (Input.GetMouseButtonDown(0) && !isHovering && !EventSystem.current.IsPointerOverGameObject())
        {
            gameObject.SetActive(false);
        }
    }
    public void UpdateRouteText(bool toggle)
    {
        //Set routetext to new routenames
        if (toggle) routeText.text = routenames[0];
        else routeText.text = routenames[1];

    }
    public void SelectObject(MapUnitData data, MapUnit mapunit, float rotation)
    {
        //set mapunit to selectedObject
        selectedObject = mapunit;
        if (!gameObject.activeSelf || mapunit.gameObject != selectedObject.gameObject)
        {
            //set all ui elements to data from unit data - update route
            gameObject.SetActive(true);
            idtext.text = "ID: [" + data.id.ToString() + "]";
            nametext.text = "[" + data.name + "]";
            iconimage.sprite = data.icon;
            rotationSlider.value = rotation;
            UpdateRouteText(!mapunit.hasRoute);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    public void ToggleRoute()
    {
        //Toggle route
        selectedObject.ToggleRoute();
    }
    public void HidePanel()
    {
        //Hide panel
        gameObject.SetActive(false);
    }
    public void RotateObject(float angle)
    {
        //Rotate object
        selectedObject.Rotate(angle);
    }
}
