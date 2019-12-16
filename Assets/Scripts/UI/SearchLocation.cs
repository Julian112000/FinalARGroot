using Mapbox.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SearchLocation : MonoBehaviour
{
    public static SearchLocation Instance;

    [SerializeField]
    private GameObject savedLocationPrefab;
    [SerializeField]
    private Transform savedlocationContent;

    [SerializeField]
    private ForwardGeocodeUserInput geouserinput;
    [SerializeField]
    private InputField inputfield;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        for (int i = 0; i < 100; i++)
        {
            DataSender.Instance.OnLoadLocation(i);
        }
    }
    public void Like()
    {
        SavedLocation savedlocation = Instantiate(savedLocationPrefab, savedlocationContent).GetComponent<SavedLocation>();
        savedlocation.Initialize(geouserinput.Coordinate, inputfield.text);
        DataSender.Instance.OnSaveLocation(inputfield.text, geouserinput.Coordinate.x, geouserinput.Coordinate.y);
    }
    public void AddLocation(string name, double longitude, double latitude)
    {
        SavedLocation savedlocation = Instantiate(savedLocationPrefab, savedlocationContent).GetComponent<SavedLocation>();
        savedlocation.Initialize(new Mapbox.Utils.Vector2d(longitude, latitude), name);
    }

}
