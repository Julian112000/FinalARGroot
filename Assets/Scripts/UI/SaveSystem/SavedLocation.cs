using Mapbox.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Mapbox.Unity.Map;

public class SavedLocation : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    private Text placetext;

    private string name;
    private Vector2d location;

    public void Initialize(Vector2d location, string name)
    {
        this.name = name;
        this.location = location;
        placetext.text = name;
    }
    public void OnPointerDown(PointerEventData Data)
    {
        AbstractMap.Instance.UpdateMap(location);
    }
    public void UnLike()
    {
        DataSender.Instance.OnDeleteLocation(name);
        Destroy(this.gameObject);
    }
}
