using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    public void CloseStartScreen()
    {
        animator.SetTrigger("FadeOut");
    }

    public void SwitchScene()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitApp()
    {
        Application.Quit();
    }
}
