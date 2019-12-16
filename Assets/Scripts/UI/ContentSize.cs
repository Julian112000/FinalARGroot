using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContentSize : MonoBehaviour
{
    private RectTransform rect;

    [SerializeField]
    private RectTransform content;

    float originalYsize;
    float newYsize;
    float spacing;

    private void Start()
    {
        rect = gameObject.GetComponent<RectTransform>();

        MapUnitDataToText[] allContent;
        allContent = gameObject.GetComponentsInChildren<MapUnitDataToText>();

        if (allContent.Length > 1)
        {
            originalYsize = allContent[0].gameObject.GetComponent<RectTransform>().sizeDelta.y;

            foreach (MapUnitDataToText con in allContent)
            {
                newYsize += con.gameObject.GetComponent<RectTransform>().sizeDelta.y;
                spacing += content.GetComponent<VerticalLayoutGroup>().spacing;
            }
        }

        ChangeContentSize();
    }


    private void ChangeContentSize()
    {
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, rect.sizeDelta.y + (newYsize - originalYsize) + spacing);
    }
}
