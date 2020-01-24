using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenFavTab : MonoBehaviour
{

    [SerializeField]
    private GameObject favTab;

    [SerializeField]
    private GameObject favArrow, favArrowDown;

    public void OpenFavoriteTab()
    {
        if (!favTab.activeSelf)
        {
            favTab.SetActive(true);
            favArrow.SetActive(false);
            favArrowDown.SetActive(true);
        }
        else
        {
            favTab.SetActive(false);
            favArrow.SetActive(true);
            favArrowDown.SetActive(false);
        }
    }
}
