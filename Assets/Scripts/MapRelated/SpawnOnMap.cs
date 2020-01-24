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
        //Instance of the script
        public static SpawnOnMap Instance;

        //All modeldatas
        [SerializeField]
        private List<MapUnitData> models = new List<MapUnitData>();
        //reference to the main map component
		[SerializeField]
		private AbstractMap _map;
        //Size of spawned objects [100 = normal, 1000 = huge]
		[SerializeField]
		private float spawnScale = 100f;
        //list of all spawnedobjects in the simulation
        [SerializeField]
		private List<GameObject> spawnedObjects;
        //list of all vector2d (latitude, longitude) locations
        [SerializeField]
        public List<Vector2d> locations;

        private int currentid;          //Current Data of map 
        private string currentname;     //Current name of the map  
        private string currentdate;     //Current date when the map was last saved
        private int routeamount;        //Amount of routes 

        void Start()
		{
            //create new spawnedobjects + locations
            Instance = this;
			spawnedObjects = new List<GameObject>();
            locations = new List<Vector2d>();
		}
        private void Update()
        {
            //set local count variable to the spawnedobjects amount
            int count = spawnedObjects.Count;
            for (int i = 0; i < count; i++)
            {
                //spawn objects with give data
                GameObject spawnedObject = spawnedObjects[i];
                //set location to the right location in the list
                Vector2d location = locations[i];
                spawnedObject.transform.localPosition = _map.GeoToWorldPosition(location, true);        //set localposition from the worldposition
                spawnedObject.transform.localScale = new Vector3(spawnScale, spawnScale, spawnScale);   //set localscale to the spawn scale set above.
            }
        }
        //One of the first voids to call when saved units
        public void SaveUnits(string name)
        {
            //loop trough list of spawnedobjects in the simulation
            for (int i = 0; i < spawnedObjects.Count; i++)
            {
                //save all mapunits to the database, with given name, amount etc.
                DataSaver.SaveMapUnits(spawnedObjects[i], name, i);
            }
        }
        //Void to change the amount of routes in the simulation
        public void SetRouteAmount(int amount)
        {
            //add the local amount to the global one
            routeamount += amount;
        }
        //Void to add an object
        public void AddObject(GameObject newobject, Vector2d location)
        {
            //add the newobject variable to the spawnedobject list, same for it's location
            spawnedObjects.Add(newobject);
            locations.Add(location);
        }
        //Most important function to load the scenario
        public void Load(int number, string name, string time, int amount)
        {
            //set all local variables to the global ones
            currentid = number; //id
            currentname = name; //name
            currentdate = time; //date

            //loop trough all units in the world and destroy them
            foreach (MapUnit unit in FindObjectsOfType(typeof(MapUnit)))
            {
                spawnedObjects.Remove(unit.gameObject);
                Destroy(unit.gameObject);
            }
            //loop trought the amount variable from the database unit 
            for (int i = 0; i < amount; i++)
            {
                //send the local variables to the database function
                DataSender.Instance.OnLoadUnits(number, i, routeamount);
            }
            //
        }
        #region ReturnTypes
        //Return spawnedobjects amount
        public int GetUnitAmount()
        {
            return spawnedObjects.Count;
        }
        //Return Routeamounts
        public int GetRouteAmount()
        {
            return routeamount;
        }
        //Return main Unit Order
        int GetUnitOrder(GameObject unit)
        {
            int order = -1;
            //loop trough spawnedobjects amount
            for (int i = 0; i < spawnedObjects.Count; i++)
            {
                if (spawnedObjects[i] == unit)
                {
                    order = i;
                }
            }
            return order;
        }
        #endregion

        //Main function to load units
        public void LoadUnit(int id, int teamid, Vector2d location, float altitude, int rotation)
        {
            //Call the 'SpawnObject' function with local variables
            SpawnObject(teamid, models[id].model[teamid], location, altitude, rotation);
        }
        //Main function to spawn the new units
        public void SpawnObject(int team, GameObject type, Vector2d location, float altitude, int rotation)
        {
            //Create an instance from the instantiated empty object
            MapUnit instance = Instantiate(type).GetComponent<MapUnit>();
            //set all the orientation points
            instance.transform.localPosition = _map.GeoToWorldPosition(location, true);         //Position
            instance.transform.localScale = new Vector3(spawnScale, spawnScale, spawnScale);    //Scale
            instance.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));              //Rotation
            //Initialize the data to the new instance
            instance.Initialize(team, location.x, location.y, altitude, rotation);
            //add everything to the lists
            spawnedObjects.Add(instance.gameObject);
            locations.Add(location);
        }
        //Main function to delete an unit
        public void DeleteObject()
        {
            //set the instance inside the scanario to invinseble
            ObjectSelector.Instance.selectedObject.gameObject.SetActive(false);

            //delete all map units with the database call function
            DataSender.Instance.DeleteMapUnits(GetUnitOrder(ObjectSelector.Instance.selectedObject.gameObject));
            //DELETE FROM DATABASE
        }
    }
}