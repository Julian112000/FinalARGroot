using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScenarioPanel : MonoBehaviour
{
    //Gameobject that will activate when hovered over the UI bar
    [SerializeField]
    private GameObject loadbar;
    //Text component where the name of the scenario will take place
    [SerializeField]
    private Text nameText;
    //Text component where the date of the scenario will take place
    [SerializeField]
    private Text dateText;

    private int id;         // id of the scenario created local and uploaded to database when loaded
    private string name;    // name of the scenario *from database when loaded*
    private string date;    // date of the scenario *from database when loaded*

    //Function to initialize the scenario UI panel
    public void Initialize(int id, string name, string date)
    {
        this.id = id;           //set id from initialized constructor
        this.name = name;       //set name from initialized constructor
        this.date = date;       //set data from initialized constructor
        nameText.text = name;   //set name text component 
        dateText.text = date;   //set date text component 
    }
    //function to call when hovered over
    public void ShowLoadBar(bool load)
    {
        //activate loadbar on and off
        loadbar.SetActive(load);
    }
    //function to load scenario
    public void LoadScenario()
    {
        //database function to load a scenario
        DataSender.Instance.OnLoadScenario(id);
    }
}
