using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SearchModel : MonoBehaviour
{
    public MapUnitData[] models;

    [SerializeField]
    private List<string> names;

    [SerializeField]
    private GameObject searchBar;

    private string searchInput;

    [SerializeField]
    private GameObject resultPrefab, resultContent;
    
    [SerializeField]
    private List<MapUnitDataToText> results;
    [SerializeField]
    private List<string> foundnames = new List<string>();

    private void Awake()
    {
        for (int i = 0; i < models.Length; i++)
        {
            names.Add(models[i].name);
            names[i] = names[i].ToLower();
        }
    }
    public void Update()
    {
        searchInput = searchBar.GetComponent<Text>().text;
        searchInput = searchInput.ToLower();

        for (int i = 0; i < names.Count; i++)
        {
            if (string.IsNullOrEmpty(searchInput))
            {
                foundnames.Clear();
                for (int j = 0; j < results.Count; j++)
                {
                    string modelname = results[i].model.name.ToLower();
                    foundnames.Remove(modelname);
                    Destroy(results[i].gameObject);
                    results.Remove(results[i]);
                }
                return;
            }
            else if (names[i].Contains(searchInput))
            {
                //Debug.Log(names[i]);
                if (!foundnames.Contains(names[i]))
                {
                    foundnames.Add(names[i]);
                    
                    AddResults(names[i], i);
                }
            }
            else if (!names[i].Contains(searchInput))
            {
                UpdateResults(i);
            }
        }
    }
    void AddResults(string name, int id)
    {
        GameObject result = Instantiate(resultPrefab) as GameObject;
        result.transform.SetParent(resultContent.transform, false);
        result.transform.GetComponent<MapUnitDataToText>().SetModel(models[id]);
        results.Add(result.transform.GetComponent<MapUnitDataToText>());
    }
    void UpdateResults(int id)
    {
        Debug.Log(id);
        for (int i = 0; i < results.Count; i++)
        {
            if (results[i].model.id == id)
            {
                string modelname = results[i].model.name.ToLower();
                foundnames.Remove(modelname);
                Destroy(results[i].gameObject);
                results.Remove(results[i]);
            }
        }
    }

}

