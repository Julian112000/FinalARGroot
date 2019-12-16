using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationHandler : MonoBehaviour
{
    [SerializeField]
    private Animator leftBarAnimator;

    [SerializeField]
    private GameObject searchBar;

    void Update()
    {
        OpenSearchResults();
    }

    void OpenSearchResults()
    {
        if (searchBar.GetComponent<InputField>().isFocused)
        {
            if(searchBar.GetComponent<InputField>().text != "")
            {
                leftBarAnimator.SetBool("ShowResult", true);
            }
            else
            {
                leftBarAnimator.SetBool("ShowResult", false);
            }
        }
        else
        {
            if(searchBar.GetComponent<InputField>().text == "")
            {
                leftBarAnimator.SetBool("ShowResult", false);
            }
        }
    }
}
