using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Examples;

public class ScenarioEditor : MonoBehaviour
{
    public static ScenarioEditor Instance;

    [SerializeField]
    private GameObject scenarioPanel;
    [SerializeField]
    private Transform scenarioTransform;

    private List<int> sceneids = new List<int>();

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        DataSaver.LoadScenarios(gameObject, 100);
    }
    public void CreateNew()
    {
        DataSender.Instance.OnSaveMapScenario("scene " + Random.Range(0, 999), SpawnOnMap.Instance.GetUnitAmount(), SpawnOnMap.Instance.GetRouteAmount());
    }

    public void CreateNewPanel(int id, string name, string date)
    {
        if (!sceneids.Contains(id))
        {
            ScenarioPanel panel = Instantiate(scenarioPanel, scenarioTransform.transform).GetComponent<ScenarioPanel>();
            panel.Initialize(id, name, date);
            sceneids.Add(id);
        }
    }
    public void LoadScenarios()
    {
        DataSaver.LoadScenarios(gameObject, 100);
    }
    public void Delete()
    {

    }
    public void Load()
    {
        gameObject.SetActive(false);
    }
}
