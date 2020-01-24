using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapUnitDataToText : MonoBehaviour
{
    //Mapunit Data 
    public MapUnitData model;

    public Text idUI;       //Text component of id number
    public Text nameUI;     //Text component of name string
    public Image iconUI;    //Image component of icon image


    //bool to check
    [SerializeField]
    private bool isSearched;

    private void Awake()
    {
        if (!isSearched)
        {
            idUI.text = "ID: " + model.id;
            nameUI.text = "" + model.name;
            iconUI.sprite = model.icon;
        }
    }
    public void SetModel(MapUnitData newmodel)
    {
        model = newmodel;
        nameUI.text = newmodel.name;
        iconUI.sprite = newmodel.icon;
    }
    public void Selected()
    {
        ObjectSelector.Instance.SelectModel(this);
    }
}
