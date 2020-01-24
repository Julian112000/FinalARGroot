using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentSelectedModel : MonoBehaviour
{
    public static CurrentSelectedModel Instance;

    public MapUnitData currentData;

    [SerializeField]
    private Image currentModelImage;
    [SerializeField]
    private Text currentModelName;

    private void Awake()
    {
        Instance = this;
    }
    public void UpdateUI(MapUnitDataToText data)
    {
        currentData = data.model;
        currentModelName.text = data.model.name;
        currentModelImage.sprite = data.model.icon;
    }
}
