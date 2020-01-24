using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveButton : MonoBehaviour
{
    [SerializeField]
    private GameObject saveCanvas, normalCanvas;


    public void saveCanvasOpen()
    {
        if (!saveCanvas.activeSelf)
        {
            saveCanvas.SetActive(true);
            normalCanvas.GetComponent<Animator>().SetTrigger("CloseBar");
        }
        else
        {
            return;
        }
    }

    public void saveCanvasClose()
    {
        normalCanvas.SetActive(true);
        saveCanvas.GetComponent<Animator>().SetTrigger("CloseSave");
    }
}
