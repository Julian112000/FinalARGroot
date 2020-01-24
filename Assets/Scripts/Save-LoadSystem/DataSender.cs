using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Examples;
using Mapbox.Utils;

public class DataSender : MonoBehaviour
{
    //get instance from the static script
    public static DataSender Instance;

    //Main connection URL
    [SerializeField]
    private string url;

    private void Awake()
    {
        //set instance to this object
        Instance = this;
    }
    //main function to save the scenario
    public void OnSaveMapScenario(string name, int unitamount, int routeamount)
    {
        StartCoroutine(SaveMapScenario(name, unitamount, routeamount));
    }
    //Save map units function
    public void OnSaveMapUnits(string scenename, int order, int id, int teamid, double latitude, double longitude, double altitude, int rotation, List<Vector2d> waypoints)
    {
        StartCoroutine(SaveMapUnits(scenename, order, id, teamid, latitude, longitude, altitude, rotation, waypoints));
    }
    //Load all scenarios with a given id
    public void OnLoadScenario(int id)
    {
        StartCoroutine(LoadScenario(id));
    }
    //Load units with number of order, id and waypointcount
    public void OnLoadUnits(int number, int number2, int routenumber)
    {
        StartCoroutine(GetUnits(number, number2, routenumber));
    }
    //function to delete map units
    public void DeleteMapUnits(int order)
    {
        StartCoroutine(RemoveMapUnits(order));
    }
    //select a scenario and load the specific one
    public void OnSelectScenario(int id)
    {
        StartCoroutine(LoadScenarios(id));
    }
    //Save location stored in a location function
    public void OnSaveLocation(string name, double lat, double lon)
    {
        StartCoroutine(SaveLocation(name, lat, lon));
    }
    //Save waypoints of the unit
    public void OnSaveWaypoints(string name, int unitid, int waypointid, Vector2d positions)
    {
        StartCoroutine(SaveMapWaypoints(name, unitid, waypointid, positions.x, positions.y));
    }
    //load location 
    public void OnLoadLocation(int id)
    {
        StartCoroutine(LoadLocations(id));
    }
    //delete location
    public void OnDeleteLocation(string name)
    {
        StartCoroutine(DeleteLocation(name));
    }

    //IEnumerator to delete a location
    IEnumerator DeleteLocation(string name)
    {
        //create a new www connection
        WWWForm form = new WWWForm();
        //add variables to the wwwform to use in the php scripts later
        form.AddField("action", "delete");
        form.AddField("name", name);
        //set new connection with .php callback
        WWW www = new WWW(url + "locationapi.php", form);
        //return www
        yield return www;
        if (www.text != "error")
        {
            //data to call when there is no error
        }
    }
    //IEnumerator to load all locations 
    IEnumerator LoadLocations(int id)
    {
        //create wwwform connection
        WWWForm form = new WWWForm();
        //add the variables to the www-form
        form.AddField("action", "load");
        form.AddField("id", id);
        //set new connection with new callback with the right php scripts
        WWW www = new WWW(url + "locationapi.php", form);
        //return www
        yield return www;
        
        //check if www is not giving an error
        if (www.text != "error")
        {
            //store all data to a local variable
            string data = www.text;
            //split variables and store in values variable
            string[] values = data.Split(";"[0]);

            //set local variables from the split variables
            string name = values[0];            
            double lat = double.Parse(values[1]);
            double lon = double.Parse(values[2]);
            //call function to add location with the given data
            SearchLocation.Instance.AddLocation(name, lat, lon);
        }
    }
    //IEnumerator to save locations
    IEnumerator SaveLocation(string name, double latitude, double longitude)
    {
        //create new connection
        WWWForm form = new WWWForm();
        //add specific variables to the wwwform to use in the php scripts later
        form.AddField("action", "create");                  
        form.AddField("name", name);                        //add name of the location
        form.AddField("latitude", latitude.ToString());     //add latitude to the field
        form.AddField("longitude", longitude.ToString());   //add longitude to the field
        //add new www with the right .php callback
        WWW www = new WWW(url + "locationapi.php", form);
        yield return www;
        if (www.text != "error")
        {
            //data to call when there is no error
        }
    }
    //IEnumerator to get all units and load them
    IEnumerator GetUnits(int sceneid, int id, int routeamount)
    {
        //create new wwwform
        WWWForm form = new WWWForm();
        //add specific variables to the wwwform to use in the php scripts later
        form.AddField("action", "unit");        
        form.AddField("sceneid", sceneid);  //add the sceneid to the field
        form.AddField("order", id);         //add the order to the field
        //add new www with the right .php callback
        WWW www = new WWW(url + "loadapi.php", form);
        //return the www
        yield return www;
        if (www.text != "error")
        {
            //store www.text to the local data string
            string data = www.text;
            //split variables and store in values variable
            string[] values = data.Split(";"[0]);

            int db_id = int.Parse(values[0]);       //get id from values
            int db_team = int.Parse(values[1]);     //get teamid from values [DEFAULT: 0 = allie, 1 = opfor]
            double lat = double.Parse(values[2]);   //get latitude (x)
            double lon = double.Parse(values[3]);   //get longitude (z)
            float alt = float.Parse(values[4]);     //get altitude (y)
            int rot = int.Parse(values[5]);         //get rotation from database
            //Call function with data above
            SpawnOnMap.Instance.LoadUnit(db_id, db_team, new Vector2d(lat, lon), alt, rot);
        }
    }
    //IEnumerator to load scenario with sceneid
    IEnumerator LoadScenario(int sceneid)
    {
        //create new wwwform connection
        WWWForm form = new WWWForm();
        //add specific variables to the wwwform to use in the php scripts later
        form.AddField("action", "scenario");
        form.AddField("sceneid", sceneid);      //add sceneid to the field
        //add new www with the right .php callback
        WWW www = new WWW(url + "loadapi.php", form);
        //return www
        yield return www;
        //when the www return is not given an error
        if (www.text != "error")
        {
            //store www.text to the local data variable
            string data = www.text;
            //split variables and store in values variable
            string[] values = data.Split(";"[0]);

            string db_name = values[0];             //set first value to the local string
            string db_time = values[1];             //set time value to the local string
            int db_amount = int.Parse(values[2]);   //get amount from the database
            //call load function with data above
            SpawnOnMap.Instance.Load(sceneid, db_name, db_time, db_amount);
        }
    }
    //IEnumerator to load all scenarios as an UI panel
    IEnumerator LoadScenarios(int id)
    {
        //create new wwwform connection
        WWWForm form = new WWWForm();
        //add specific variables to the wwwform to use in the php scripts later
        form.AddField("action", "load");
        form.AddField("sceneid", id);       //add id to the sceneid variable
        //add new www with the right .php callback
        WWW www = new WWW(url + "scenarioapi.php", form);
        //return www callback
        yield return www;
        //when www return type is not empty
        if (www.text != "")
        {
            //store www return to the local variable
            string data = www.text;
            //split variables and store in values variable
            string[] values = data.Split(","[0]);

            int db_id = int.Parse(values[0]);   //local id variable to get first value from database
            string db_name = values[1];         //get name of the scenario and store it
            string db_time = values[2];         //get time when scenario was created from database and store it

            //call function to create an UI panel for the scenario data above.
            ScenarioEditor.Instance.CreateNewPanel(db_id, db_name, db_time);
        }
    }
    //IEnumerator to save main function *biggest call*
    IEnumerator SaveMapUnits(string scenename, int order, int id, int teamid, double latitude, double longitude, double altitude, int rotation, List<Vector2d> waypoints)
    {
        //create new wwwform connection
        WWWForm form = new WWWForm();
        //add specific variables to the wwwform to use in the php scripts later
        form.AddField("action", "create");
        form.AddField("scenename", scenename);              //add the name of the scene to the field
        form.AddField("type", id);                          //add the type of the unit to the field [DEFAULT: 0 = fennek, 1 = Soldier etc.]
        form.AddField("team", teamid);                      //add the team to the field [DEFAULT: 0 = allie, 1 = opfor]
        form.AddField("latitude", latitude.ToString());     //add latitude to the field (x)
        form.AddField("longitude", longitude.ToString());   //add longitude to the field (z)
        form.AddField("altitude", altitude.ToString());     //add altitude to the field (y)
        form.AddField("rotation", rotation);                //orientation of the unit
        form.AddField("idcount", order);                    //id to get the right unit in order in the database
        //add new www with the right .php callback
        WWW www = new WWW(url + "unitapi.php", form);
        //return www
        yield return www;
        //when www callback is not giving any errors
        if (www.text != "error")
        {
            //get all waypoints from the unit
            for (int i = 0; i < waypoints.Count; i++)
            {
                //call function to save the waypoint and start the process
                OnSaveWaypoints(scenename, order, i, waypoints[i]);
            }
        }
    }
    //IEnumerator to save the unit waypoints
    IEnumerator SaveMapWaypoints(string scenename, int unitid, int waypointid, double latitude, double longitude)
    {
        //create new wwwform connection
        WWWForm form = new WWWForm();
        //add specific variables to the wwwform to use in the php scripts later
        form.AddField("action", "create");
        form.AddField("scenename", scenename);              //name of scenario that the unit was in
        form.AddField("unitid", unitid);                    //unit id of the waypoints 
        form.AddField("waypointid", waypointid);            //current waypointid [can go up to 100]
        form.AddField("latitude", latitude.ToString());     //latitude of that waypoint [x]
        form.AddField("longitude", longitude.ToString());   //longitude of that waypoint [z]
        //add new www with the right .php callback
        WWW www = new WWW(url + "waypointapi.php", form);
        //return www
        yield return www;
    }
    //IEnumerator to save main scenario *important*
    IEnumerator SaveMapScenario(string name, int unitamount, int routeamount)
    {
        //create new wwwform connection
        WWWForm form = new WWWForm();
        //add specific variables to the wwwform to use in the php scripts later
        form.AddField("action", "create");
        form.AddField("name", name);                        //add name to the field
        form.AddField("data", DateTime.Now.ToString());     //add time when scenario was created
        form.AddField("unitamount", unitamount);            //add total amount of units in the scenario
        form.AddField("routeamount", routeamount);          //add amount of routes/waypoints in scenario
        //add new www with the right .php callback
        WWW www = new WWW(url + "scenarioapi.php", form);
        //return www
        yield return www;
        //when www callback is not giving an error
        if (www.text != "error")
        {
            //load all ui scenarios and start saving the units
            ScenarioEditor.Instance.LoadScenarios();
            SpawnOnMap.Instance.SaveUnits(name);
        }
    }
    //IEnumerator to remove all map units from database
    IEnumerator RemoveMapUnits(int order)
    {
        //create wwwform connection
        WWWForm form = new WWWForm();
        //add specific variables to the wwwform to use in the php scripts later
        form.AddField("action", "removal");
        form.AddField("idcount", order);    //add order variable to the field
        //add new www with the right .php callback
        WWW www = new WWW(url + "deleteapi.php", form);
        //return www
        yield return www;
    }
}
