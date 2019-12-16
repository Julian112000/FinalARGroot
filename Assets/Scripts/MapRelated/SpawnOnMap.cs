namespace Mapbox.Examples
{
	using UnityEngine;
	using Mapbox.Utils;
	using Mapbox.Unity.Map;
	using Mapbox.Unity.MeshGeneration.Factories;
	using Mapbox.Unity.Utilities;
	using System.Collections.Generic;

	public class SpawnOnMap : MonoBehaviour
	{
        public static SpawnOnMap Instance;

        //All modeldatas
        [SerializeField]
        private List<MapUnitData> models = new List<MapUnitData>();
		[SerializeField]
		private AbstractMap _map;
        //Size of spawned objects
		[SerializeField]
		private float spawnScale = 100f;
        [SerializeField]
		private List<GameObject> spawnedObjects;
        [SerializeField]
        public List<Vector2d> locations;

        //Current Data of map
        private int currentid;
        private string currentname;
        private string currentdate;
        private int routeamount;

        void Start()
		{
            Instance = this;
			spawnedObjects = new List<GameObject>();
            locations = new List<Vector2d>();
		}
        private void Update()
        {
            int count = spawnedObjects.Count;
            for (int i = 0; i < count; i++)
            {
                GameObject spawnedObject = spawnedObjects[i];
                Vector2d location = locations[i];
                spawnedObject.transform.localPosition = _map.GeoToWorldPosition(location, true);
                spawnedObject.transform.localScale = new Vector3(spawnScale, spawnScale, spawnScale);
            }
        }
        public void SaveUnits(string name)
        {
            for (int i = 0; i < spawnedObjects.Count; i++)
            {
                DataSaver.SaveMapUnits(spawnedObjects[i], name, i);
            }
        }
        public void SetRouteAmount(int amount)
        {
            routeamount += amount;
        }
        public void AddObject(GameObject newobject, Vector2d location)
        {
            spawnedObjects.Add(newobject);
            locations.Add(location);
        }
        public void Load(int number, string name, string time, int amount)
        {
            currentid = number;
            currentname = name;
            currentdate = time;

            foreach (MapUnit unit in FindObjectsOfType(typeof(MapUnit)))
            {
                spawnedObjects.Remove(unit.gameObject);
                Destroy(unit.gameObject);
            }
            for (int i = 0; i < amount; i++)
            {
                DataSender.Instance.OnLoadUnits(number, i, routeamount);
            }
            //
        }
        public int GetUnitAmount()
        {
            return spawnedObjects.Count;
        }
        public int GetRouteAmount()
        {
            return routeamount;
        }
        public void LoadUnit(int id, int teamid, Vector2d location, float altitude, int rotation)
        {
            SpawnObject(teamid, models[id].model[teamid], location, altitude, rotation);
        }
        public void SpawnObject(int team, GameObject type, Vector2d location, float altitude, int rotation)
        {
            MapUnit instance = Instantiate(type).GetComponent<MapUnit>();
            //rotation
            instance.transform.localPosition = _map.GeoToWorldPosition(location, true);
            instance.transform.localScale = new Vector3(spawnScale, spawnScale, spawnScale);
            instance.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
            //data
            instance.Initialize(team, location.x, location.y, altitude, rotation);
            //
            spawnedObjects.Add(instance.gameObject);
            locations.Add(location);

            //ADD NEW ID TO DATABASE AS A NEW TABLE
            //SET ID TO THE ROW
        }
        public void DeleteObject()
        {
            ObjectSelector.Instance.selectedObject.gameObject.SetActive(false);

            DataSender.Instance.DeleteMapUnits(GetUnitOrder(ObjectSelector.Instance.selectedObject.gameObject));
            //DELETE FROM DATABASE
        }
        int GetUnitOrder(GameObject unit)
        {
            int order = -1;
            for (int i = 0; i < spawnedObjects.Count; i++)
            {
                if (spawnedObjects[i] == unit)
                {
                    order = i;
                }
            }
            return order;
        }
    }
}