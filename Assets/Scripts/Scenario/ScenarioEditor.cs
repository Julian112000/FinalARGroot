using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Examples;

public class ScenarioEditor : MonoBehaviour
{
    //Instance of this script / Direct call
    public static ScenarioEditor Instance;

    //Main scenario UI panel to instantiate
    [SerializeField]
    private GameObject scenarioPanel;
    //parent that the scenariopanel will be instantiated in.
    [SerializeField]
    private Transform scenarioTransform;
    //list of all existing scene ids
    private List<int> sceneids = new List<int>();

    private void Awake()
    {
        //set instance to this script
        Instance = this;
    }
    private void Start()
    {
        //load ui scenarios
        DataSaver.LoadScenarios(gameObject, 100);
    }
    //main function to create new saving
    public void CreateNew()
    {
        //Call function to save new scenario with given data
        DataSender.Instance.OnSaveMapScenario("scene " + Random.Range(0, 999), SpawnOnMap.Instance.GetUnitAmount(), SpawnOnMap.Instance.GetRouteAmount());
    }
    //function to create new UI panel for the scenario
    public void CreateNewPanel(int id, string name, string date)
    {
        //when this scenario doesnt contain in the list
        if (!sceneids.Contains(id))
        {
            //instantiate panel
            ScenarioPanel panel = Instantiate(scenarioPanel, scenarioTransform.transform).GetComponent<ScenarioPanel>();
            //initialize when panel was instantiated
            panel.Initialize(id, name, date);
            //add the id of the scenario to the list
            sceneids.Add(id);
        }
    }
    //Function to load all the scenarios
    public void LoadScenarios()
    {
        DataSaver.LoadScenarios(gameObject, 100);
    }
    //First function that will be called when clicked on the delete button
    public void Delete()
    {

    }
    //First function that will be called when clicked on the load button
    public void Load()
    {
        gameObject.SetActive(false);
    }
}
