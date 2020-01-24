using System.Collections.Generic;
using UnityEngine;
using System;
using Mapbox.Utils;

public static class DataSaver
{
    //Main Function to save map units and upload to the database
    public static void SaveMapUnits(GameObject unit, string scenename, int id)
    {
        //create new mapunitdata as a new gameobject
        MapUnitData mapunit = new MapUnitData(unit);
        //Call the main function to save the map units
        DataSender.Instance.OnSaveMapUnits(scenename, id, mapunit.id, mapunit.teamid, mapunit.latitude, mapunit.longitude, mapunit.altitude, mapunit.rotation, mapunit.positions);
    }

    //Function to load the scenarios as UI scenarios
    public static void LoadScenarios(GameObject sceneinfo, int number)
    {
        for (int i = 0; i < number; i++)
        {
            //load all scenarios with amount of given scenarios [default = 100]
            DataSender.Instance.OnSelectScenario(i);
        }
    }
    //create serializable class to load the unit data
    [Serializable]
    public class MapUnitData
    {
        public int id;              // id to determain the order in database
        public int teamid;          // teamid [0 = allie, 1 = opfor]
        public double latitude;     // latitude coordinate (x)
        public double longitude;    // longitude coordinate (z)
        public float altitude;      // altitude coordinate (y)
        public int rotation;        // rotation of the object (360 degrees)
        public List<Vector2d> positions = new List<Vector2d>(); // list of all the waypoint positions

        public MapUnitData(GameObject unit)
        {
            //get mapdata from its component
            MapUnit mapdata = unit.GetComponent<MapUnit>();

            id = mapdata.GetData().id;          //set id of the mapdata
            teamid = mapdata.teamid;            //set teamid of the mapdata
            latitude = mapdata.latitude;        //set latitude from the local variable
            longitude = mapdata.longitude;      //set longitude from the local variable
            altitude = mapdata.altitude;        //set altitude from the local variable
            rotation = (int)mapdata.rotation;   //set rotation of the local variable rotation

            //check if mapdata has a route
            if (mapdata.hasRoute)
                //set the positions to the positions of all the waypoints
                positions = mapdata.GetRoute().GetPositions();
        }
    }
}
