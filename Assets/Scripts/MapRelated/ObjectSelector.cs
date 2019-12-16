using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ObjectSelector : MonoBehaviour
{
    public static ObjectSelector Instance;

    public MapUnit selectedObject;
    public bool isHovering;

    //Visualisation
    [SerializeField]
    private Text nametext;
    [SerializeField]
    private Text idtext;
    [SerializeField]
    private Image iconimage;
    [SerializeField]
    private Slider rotationSlider;

    //Route
    [SerializeField]
    private string[] routenames;
    [SerializeField]
    private Text routeText;

    private void Awake()
    {
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
