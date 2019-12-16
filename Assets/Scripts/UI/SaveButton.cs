using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveButton : MonoBehaviour
{
    [SerializeField]
    private GameObject saveCanvas;


    public void saveCanvasOpen()
    {
        if (!saveCanvas.activeSelf)
        {
            saveCanvas.SetActive(true);
        }
        else
        {
            return;
        }
    }

    public void saveCanvasClose()
    {
        saveCanvas.SetActive(false);
    }
}
