using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mapbox.Utils;

public static class DataSaver
{
    public static void SaveMapUnits(GameObject unit, string scenename, int id)
    {
        MapUnitData mapunit = new MapUnitData(unit);
        DataSender.Instance.OnSaveMapUnits(scenename, id, mapunit.id, mapunit.teamid, mapunit.latitude, mapunit.longitude, mapunit.altitude, mapunit.rotation, mapunit.positions);
    }
    public static void LoadScenarios(GameObject sceneinfo, int number)
    {
        for (int i = 0; i < number; i++)
        {
            DataSender.Instance.OnSelectScenario(i);
        }
    }

    [Serializable]
    public class MapUnitData
    {
        public int id;
        public int teamid;
        public double latitude;
        public double longitude;
        public float altitude;
        public int rotation;
        public List<Vector2d> positions = new List<Vector2d>();

        public MapUnitData(GameObject unit)
        {
            MapUnit mapdata = unit.GetComponent<MapUnit>();

            id = mapdata.GetData().id;
            teamid = mapdata.teamid; 
            latitude = mapdata.latitude;
            longitude = mapdata.longitude;
            altitude = mapdata.altitude;
            rotation = (int)mapdata.rotation;
            if (mapdata.hasRoute)
                positions = mapdata.GetRoute().GetPositions();
        }
    }
}
