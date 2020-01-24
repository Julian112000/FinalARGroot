using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverButtons : MonoBehaviour
{
    [SerializeField]
    private AudioSource mySounds;
    [SerializeField]
    private AudioClip onHover;
    [SerializeField]
    private AudioClip onClick;

    public void OnHoverButton()
    {
        mySounds.PlayOneShot(onHover);
    }
    
    public void OnClickButton()
    {
        mySounds.PlayOneShot(onClick);
    }
}
