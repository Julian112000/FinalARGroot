using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapUnitDataToText : MonoBehaviour
{
    public MapUnitData model;

    public Text idUI;
    public Text nameUI;
    public Image iconUI;

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
