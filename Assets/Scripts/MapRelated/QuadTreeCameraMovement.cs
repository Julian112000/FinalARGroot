namespace Mapbox.Examples
{
	using Mapbox.Unity.Map;
	using Mapbox.Unity.Utilities;
	using Mapbox.Utils;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using System;
    using Mapbox.Map;
    using Mapbox.Unity.MeshGeneration.Data;
    using Mapbox.Unity.Location;
    using UnityEngine.UI;

    public class QuadTreeCameraMovement : MonoBehaviour
	{
        //Instance of camera movement
        public static QuadTreeCameraMovement Instance;

        //How fast the tiles will be loaded
        [SerializeField]
		[Range(1, 20)]
		public float _panSpeed = 1.0f;

        //value of how fast you can scroll up and down
		[SerializeField]
		float _zoomSpeed = 0.25f;


        //reference to the second UI camera
		[SerializeField]
		public Camera _referenceCamera;
        //reference to the mapmanager script
		[SerializeField]
		AbstractMap _mapManager;

        //bool to check on if you use degree method instead of a meter conversion
		[SerializeField]
		bool _useDegreeMethod;

		private Vector3 _origin;                                //origin of the map
		private Vector3 _mousePosition;                         //Global mouseposition to set to mouseposition
		private Vector3 _mousePositionPrevious;                 //Previous global mouseposition to set the previous mouseposition
		private bool _shouldDrag;                               //Bool to check if you should drag or not
		private bool _isInitialized = false;                    //Bool to check if map is initialized or not
		private Plane _groundPlane = new Plane(Vector3.up, 0);  //Main grouldplane of the map
		private bool _dragStartedOnUI = false;                  //Bool to check if you started to use an UI element

        //main bool to check if you are creating a route
        public bool creatingRoute;
        //object that follows the map reletative to the world map
        public Transform followObjectMouse;
        //team id [0= allie, 1 = opfor]
        [SerializeField]
        private int teamId;
        //Text component of team id
        [SerializeField]
        private Text teamText;

        void Awake()
		{
            //set instance to this script in the beginning
            Instance = this;

            //when referencecamera is 0, get the reference camera from the component
            if (null == _referenceCamera)
			{
				_referenceCamera = GetComponent<Camera>();
				if (null == _referenceCamera) { Debug.LogErrorFormat("{0}: reference camera not set", this.GetType().Name); }
			}
            //event to check if the map is initialized
            _mapManager.OnInitialized += () =>
            {
                //if so return to true
                _isInitialized = true;
            };

        }
        public void Update()
		{
            //when mouse down check the bool '_dragStartedOnUI'
			if (Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject())
			{
				_dragStartedOnUI = true;
			}
            //when the mouse is released, return to false
			if (Input.GetMouseButtonUp(0))
			{
				_dragStartedOnUI = false;
			}
            //set followobjectmouse object with mouse position on world map
            followObjectMouse.transform.localPosition = _mapManager.GeoToWorldPosition(ReturnMousePos(), true);
        }


		private void LateUpdate()
		{
            //wait till initialized otherwise return
			if (!_isInitialized) { return; }

			if (!_dragStartedOnUI)
			{
				if (Input.touchSupported && Input.touchCount > 0)
				{
                    //only handle touch when touch is supported and touch has more counts than 0
					HandleTouch();
				}
				else
				{
                    //use mouse and keyboard when touch is not supported
					HandleMouseAndKeyBoard();
				}
			}
		}
        public void ChangeTeam(int team)
        {
            //change team from 'Allie' to 'Opfor'
            teamId += team;
            if (teamId > 1)
                teamId = 0;
            else if (teamId < 0)
                teamId = 1;
            //set UI text to this teamid
            teamText.text = "" + (Team)teamId;
        }
        void HandleMouseAndKeyBoard()
		{
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                // zoom
                float scrollDelta = 0.0f;
                scrollDelta = Input.GetAxis("Mouse ScrollWheel");
                ZoomMapUsingTouchOrMouse(scrollDelta);


                //pan keyboard
                float xMove = Input.GetAxis("Horizontal");
                float zMove = Input.GetAxis("Vertical");

                //PanMapUsingKeyBoard(xMove, zMove);


                //pan mouse
                PanMapUsingTouchOrMouse();
            }
		}

		void HandleTouch()
		{
			float zoomFactor = 0.0f;
			//pinch to zoom.
			switch (Input.touchCount)
			{
				case 1:
					{
						PanMapUsingTouchOrMouse();
					}
					break;
				case 2:
					{
						// Store both touches.
						Touch touchZero = Input.GetTouch(0);
						Touch touchOne = Input.GetTouch(1);

						// Find the position in the previous frame of each touch.
						Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
						Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

						// Find the magnitude of the vector (the distance) between the touches in each frame.
						float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
						float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

						// Find the difference in the distances between each frame.
						zoomFactor = 0.01f * (touchDeltaMag - prevTouchDeltaMag);
					}
					ZoomMapUsingTouchOrMouse(zoomFactor);
					break;
				default:
					break;
			}
		}
        //Zoom function with touch or mouse elements
		void ZoomMapUsingTouchOrMouse(float zoomFactor)
		{
			float zoom = Mathf.Max(0.0f, Mathf.Min(_mapManager.Zoom + zoomFactor * _zoomSpeed, 21.0f));
			if (Math.Abs(zoom - _mapManager.Zoom) > 0.0f)
			{
                //update map relieable to zoom function
				_mapManager.UpdateMap(_mapManager.CenterLatitudeLongitude, zoom);
			}
		}

		void PanMapUsingKeyBoard(float xMove, float zMove)
		{
			if (Math.Abs(xMove) > 0.0f || Math.Abs(zMove) > 0.0f)
			{
				// Get the number of degrees in a tile at the current zoom level.
				// Divide it by the tile width in pixels ( 256 in our case)
				// to get degrees represented by each pixel.
				// Keyboard offset is in pixels, therefore multiply the factor with the offset to move the center.
				float factor = _panSpeed * (Conversions.GetTileScaleInDegrees((float)_mapManager.CenterLatitudeLongitude.x, _mapManager.AbsoluteZoom));
				//MapLocationOptions locationOptions = new MapLocationOptions
				//{
				var latitudeLongitude = new Vector2d(_mapManager.CenterLatitudeLongitude.x + zMove * factor * 2.0f, _mapManager.CenterLatitudeLongitude.y + xMove * factor * 4.0f);
				//};
				_mapManager.UpdateMap(latitudeLongitude, _mapManager.Zoom);
			}
		}

		void PanMapUsingTouchOrMouse()
		{
			if (_useDegreeMethod)
			{
                //when degreemethod is used use Degree conversion
				UseDegreeConversion();
			}
			else
			{
                //when degreemethod is not used use Meter conversion
                UseMeterConversion();
			}
		}
        #region MeterConversion
        void UseMeterConversion()
		{
            //Spawn Object when: mouse button is released, has current data, is not pointing over other objects and is not creating route
			if (Input.GetMouseButtonUp(1) && CurrentSelectedModel.Instance.currentData && !EventSystem.current.IsPointerOverGameObject() && !creatingRoute)
			{
                //set mouseposscreen to vector3 mouseposition
				Vector3 mousePosScreen = Input.mousePosition;

				mousePosScreen.z = _referenceCamera.transform.localPosition.y;
				Vector3 pos = _referenceCamera.ScreenToWorldPoint(mousePosScreen);

                //return worldtogeoposition to latlongdelta
				Vector2d latlongDelta = _mapManager.WorldToGeoPosition(pos);
                Location location = LocationProviderFactory.Instance.DefaultLocationProvider.CurrentLocation;

                //return UnityLocation with latitude and longitude as a vector3
                GetUnityLocation(latlongDelta.x, latlongDelta.y, "OBJECT" );

                //Spawn object on map with rithgt model, position, zoom and height
                SpawnOnMap.Instance.SpawnObject(teamId, CurrentSelectedModel.Instance.currentData.model[teamId], latlongDelta, getAltitudeHeightLevel(latlongDelta.x, latlongDelta.y, _mapManager.Zoom), 0);

                //Debug.Log("Latitude: " + latlongDelta.x + " Longitude: " + latlongDelta.y + " Altitude: " + getAltitudeHeightLevel(latlongDelta.x, latlongDelta.y, _mapManager.Zoom));
			}

            //Move over map with scroll middle mouse button and is not pointing over object
			if (Input.GetMouseButton(2) && !EventSystem.current.IsPointerOverGameObject())
			{
				Vector3 mousePosScreen = Input.mousePosition;
				//assign distance of camera to ground plane to z, otherwise ScreenToWorldPoint() will always return the position of the camera
				mousePosScreen.z = _referenceCamera.transform.localPosition.y;
				_mousePosition = _referenceCamera.ScreenToWorldPoint(mousePosScreen);

				if (_shouldDrag == false)
				{
                    //change origin relative with referencecamera and mouse position
					_shouldDrag = true;
					_origin = _referenceCamera.ScreenToWorldPoint(mousePosScreen);
				}
			}
			else
			{
                //when pointing over object, cancel dragging
				_shouldDrag = false;
			}

			if (_shouldDrag == true)
			{
                //calculate mousepositionprevious minus mouseposition
				Vector3 changeFromPreviousPosition = _mousePositionPrevious - _mousePosition;
				if (Mathf.Abs(changeFromPreviousPosition.x) > 0.0f || Mathf.Abs(changeFromPreviousPosition.y) > 0.0f)
				{
                    //set new offset with origin and mouseposition
					_mousePositionPrevious = _mousePosition;
					Vector3 offset = _origin - _mousePosition;

                    //check if abs offset is later and z is larger
					if (Mathf.Abs(offset.x) > 0.0f || Mathf.Abs(offset.z) > 0.0f)
					{
						if (null != _mapManager)
						{
							float factor = _panSpeed * Conversions.GetTileScaleInMeters((float)0, _mapManager.AbsoluteZoom) / _mapManager.UnityTileSize;
							Vector2d latlongDelta = Conversions.MetersToLatLon(new Vector2d(offset.x * factor, offset.z * factor));
							//Debug.Log("LatLong Delta : " + latlongDelta);
							Vector2d newLatLong = _mapManager.CenterLatitudeLongitude + latlongDelta;
							//MapLocationOptions locationOptions = new MapLocationOptions
							//{
							//	latitudeLongitude = String.Format("{0},{1}", newLatLong.x, newLatLong.y),
							//	zoom = _mapManager.Zoom
							//};
							_mapManager.UpdateMap(newLatLong, _mapManager.Zoom);
						}
					}
                    //set origin to mouseposition
					_origin = _mousePosition;
				}
				else
				{
                    //if hovering over an UI element return
					if (EventSystem.current.IsPointerOverGameObject())
					{
						return;
					}
                    //set latest data to positionprevious and origin
					_mousePositionPrevious = _mousePosition;
					_origin = _mousePosition;
				}
			}
		}
        #endregion
        #region DegreeConversion
        void UseDegreeConversion()
		{
            //
			if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
			{
				var mousePosScreen = Input.mousePosition;
				//assign distance of camera to ground plane to z, otherwise ScreenToWorldPoint() will always return the position of the camera
				mousePosScreen.z = _referenceCamera.transform.localPosition.y;
				_mousePosition = _referenceCamera.ScreenToWorldPoint(mousePosScreen);

				if (_shouldDrag == false)
				{
					_shouldDrag = true;
					_origin = _referenceCamera.ScreenToWorldPoint(mousePosScreen);
				}
			}
			else
			{
				_shouldDrag = false;
			}

			if (_shouldDrag == true)
			{
				Vector3 changeFromPreviousPosition = _mousePositionPrevious - _mousePosition;
				if (Mathf.Abs(changeFromPreviousPosition.x) > 0.0f || Mathf.Abs(changeFromPreviousPosition.y) > 0.0f)
				{
					_mousePositionPrevious = _mousePosition;
					Vector3 offset = _origin - _mousePosition;

					if (Mathf.Abs(offset.x) > 0.0f || Mathf.Abs(offset.z) > 0.0f)
					{
						if (null != _mapManager)
						{
							// Get the number of degrees in a tile at the current zoom level.
							// Divide it by the tile width in pixels ( 256 in our case)
							// to get degrees represented by each pixel.
							// Mouse offset is in pixels, therefore multiply the factor with the offset to move the center.
							float factor = _panSpeed * Conversions.GetTileScaleInDegrees((float)_mapManager.CenterLatitudeLongitude.x, _mapManager.AbsoluteZoom) / _mapManager.UnityTileSize;
							//MapLocationOptions locationOptions = new MapLocationOptions
							//{
							//	latitudeLongitude = String.Format("{0},{1}", _mapManager.CenterLatitudeLongitude.x + offset.z * factor, _mapManager.CenterLatitudeLongitude.y + offset.x * factor),
							//	zoom = _mapManager.Zoom
							//};
							Vector2d latitudeLongitude = new Vector2d(_mapManager.CenterLatitudeLongitude.x + offset.z * factor, _mapManager.CenterLatitudeLongitude.y + offset.x * factor);
							_mapManager.UpdateMap(latitudeLongitude, _mapManager.Zoom);
						}
					}
					_origin = _mousePosition;
				}
				else
				{
					if (EventSystem.current.IsPointerOverGameObject())
					{
						return;
					}
					_mousePositionPrevious = _mousePosition;
					_origin = _mousePosition;
				}
			}
		}
        #endregion

        //get ground plane with raycast down to get height on map elevation
        private Vector3 getGroundPlaneHitPoint(Ray ray)
		{
            //calculate distance with ground and object
			float distance;
			if (!_groundPlane.Raycast(ray, out distance)) { return Vector3.zero; }
			return ray.GetPoint(distance);
		}
        public float getAltitudeHeightLevel(double lat, double lon, float zoom)
        {
            //
            UnwrappedTileId tileIDUnwrapped = TileCover.CoordinateToTileId(new Mapbox.Utils.Vector2d(lat, lon), (int)zoom);

            //get tile
            UnityTile tile = _mapManager.MapVisualizer.GetUnityTileFromUnwrappedTileId(tileIDUnwrapped);

            //lat lon to meters because the tiles rect is also in meters
            Vector2d v2d = Conversions.LatLonToMeters(new Mapbox.Utils.Vector2d(lat, lon));
            //get the origin of the tile in meters
            Vector2d v2dcenter = tile.Rect.Center - new Mapbox.Utils.Vector2d(tile.Rect.Size.x / 2, tile.Rect.Size.y / 2);
            //offset between the tile origin and the lat lon point
            Vector2d diff = v2d - v2dcenter;

            //maping the diffetences to (0-1)
            float Dx = (float)(diff.x / tile.Rect.Size.x);
            float Dy = (float)(diff.y / tile.Rect.Size.y);

            //height in unity units
            float h = tile.QueryHeightData(Dx, Dy);

            return h;
        }
        public Vector3 GetUnityLocation(double lat, double lon, string name)
        {
            //transform (lat and lon) to vector3 unity position with relative world scale.
            Vector3 loc = Conversions.GeoToWorldPosition(lat, lon, _mapManager.CenterMercator, _mapManager.WorldRelativeScale).ToVector3xz();
            //Debug.Log(name + ": " + loc);
            return loc;
        }
        public Vector3 GetDistance(double myloclat, double myloclon, double objloclat, double objloclon)
        {
            //Get distance in between own location to object location in vector3's
            Vector3 myloc = Conversions.GeoToWorldPosition(myloclat, myloclon, _mapManager.CenterMercator, _mapManager.WorldRelativeScale).ToVector3xz();
            Vector3 objloc = Conversions.GeoToWorldPosition(myloclon, objloclat, _mapManager.CenterMercator, _mapManager.WorldRelativeScale).ToVector3xz();

            //return distance between vector3's
            Vector3 distance = objloc - myloc;
            return distance;
        }
        public Vector3 GetDistanceInMeters(Vector3 myloc, Vector3 obloc)
        {
            //transform vector3 to meters
            Vector3 distance = obloc - myloc;

            return distance;
        }
        public Vector2d ReturnMousePos()
        {
            //when clicked on map return vector3 to vector2d variable
            Vector3 mousePosScreen = Input.mousePosition;
            mousePosScreen.z = _referenceCamera.transform.localPosition.y;
            Vector3 pos = _referenceCamera.ScreenToWorldPoint(mousePosScreen);

            //set latlongdelta from worldgeoposition 
            Vector2d latlongDelta = _mapManager.WorldToGeoPosition(pos);

            return latlongDelta;
        }
    }
    
}