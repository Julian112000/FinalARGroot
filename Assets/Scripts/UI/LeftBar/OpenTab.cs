using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenTab : MonoBehaviour
{

    [SerializeField]
    private GameObject modelTab, favTab;
    [SerializeField]
    private GameObject arrow, arrowDown;
    [SerializeField]
    private GameObject favArrow, favArrowDown;

    public void OpenBuildTab()
    {
        if (modelTab.activeSelf)
        {
            modelTab.SetActive(false);
            arrow.SetActive(true);
            arrowDown.SetActive(false);
        }
        else
        {
            modelTab.SetActive(true);
            arrow.SetActive(false);
            arrowDown.SetActive(true);
        }

    }
}
