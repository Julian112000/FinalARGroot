using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team
{
    Allie,
    Opfor
};

[CreateAssetMenu(fileName = "Data", menuName = "Mapunits/unit", order = 1)]
public class MapUnitData : ScriptableObject
{
    public int id;              //id
    public string name;         //name          
    public string desc;         //description
    public Sprite icon;         //icon image
    public GameObject[] model;  //3d model
}
