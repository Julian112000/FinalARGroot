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
        public static QuadTreeCameraMovement Instance;

        [SerializeField]
        private double testlanmyloc = 52.15884d;
        [SerializeField]
        private double testlonmyloc = 5.359311d;

        [SerializeField]
		[Range(1, 20)]
		public float _panSpeed = 1.0f;

		[SerializeField]
		float _zoomSpeed = 0.25f;

		[SerializeField]
		public Camera _referenceCamera;

		[SerializeField]
		AbstractMap _mapManager;

		[SerializeField]
		bool _useDegreeMethod;

		private Vector3 _origin;
		private Vector3 _mousePosition;
		private Vector3 _mousePositionPrevious;
		private bool _shouldDrag;
		private bool _isInitialized = false;
		private Plane _groundPlane = new Plane(Vector3.up, 0);
		private bool _dragStartedOnUI = false;

        public bool creatingRoute;
        public Transform followObjectMouse;
        [SerializeField]
        private int teamId;
        [SerializeField]
        private Text teamText;

        void Awake()
		{
            Instance = this;

            if (null == _referenceCamera)
			{
				_referenceCamera = GetComponent<Camera>();
				if (null == _referenceCamera) { Debug.LogErrorFormat("{0}: reference camera not set", this.GetType().Name); }
			}
            _mapManager.OnInitialized += () =>
            {
                _isInitialized = true;
            };

        }
        public void Update()
		{
			if (Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject())
			{
				_dragStartedOnUI = true;
			}

			if (Input.GetMouseButtonUp(0))
			{
				_dragStartedOnUI = false;
			}

            followObjectMouse.transform.localPosition = _mapManager.GeoToWorldPosition(ReturnMousePos(), true);
        }


		private void LateUpdate()
		{
			if (!_isInitialized) { return; }

			if (!_dragStartedOnUI)
			{
				if (Input.touchSupported && Input.touchCount > 0)
				{
					HandleTouch();
				}
				else
				{
					HandleMouseAndKeyBoard();
				}
			}
		}
        public void ChangeTeam(int team)
        {
            teamId += team;
            if (teamId > 1)
                teamId = 0;
            else if (teamId < 0)
                teamId = 1;
            //
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

		void ZoomMapUsingTouchOrMouse(float zoomFactor)
		{
			float zoom = Mathf.Max(0.0f, Mathf.Min(_mapManager.Zoom + zoomFactor * _zoomSpeed, 21.0f));
			if (Math.Abs(zoom - _mapManager.Zoom) > 0.0f)
			{
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
				UseDegreeConversion();
			}
			else
			{
				UseMeterConversion();
			}
		}
		void UseMeterConversion()
		{
			if (Input.GetMouseButtonUp(1) && CurrentSelectedModel.Instance.currentData && !EventSystem.current.IsPointerOverGameObject() && !creatingRoute)
			{
				Vector3 mousePosScreen = Input.mousePosition;

				mousePosScreen.z = _referenceCamera.transform.localPosition.y;
				Vector3 pos = _referenceCamera.ScreenToWorldPoint(mousePosScreen);

				Vector2d latlongDelta = _mapManager.WorldToGeoPosition(pos);
                Location location = LocationProviderFactory.Instance.DefaultLocationProvider.CurrentLocation;

                GetUnityLocation(testlanmyloc, testlonmyloc, "MYLOCATION");
                GetUnityLocation(latlongDelta.x, latlongDelta.y, "OBJECT" );

                SpawnOnMap.Instance.SpawnObject(teamId, CurrentSelectedModel.Instance.currentData.model[teamId], latlongDelta, getAltitudeHeightLevel(latlongDelta.x, latlongDelta.y, _mapManager.Zoom), 0);

                //Debug.Log("Latitude: " + latlongDelta.x + " Longitude: " + latlongDelta.y + " Altitude: " + getAltitudeHeightLevel(latlongDelta.x, latlongDelta.y, _mapManager.Zoom));
			}

			if (Input.GetMouseButton(2) && !EventSystem.current.IsPointerOverGameObject())
			{
				Vector3 mousePosScreen = Input.mousePosition;
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

		void UseDegreeConversion()
		{
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

		private Vector3 getGroundPlaneHitPoint(Ray ray)
		{
			float distance;
			if (!_groundPlane.Raycast(ray, out distance)) { return Vector3.zero; }
			return ray.GetPoint(distance);
		}
        public float getAltitudeHeightLevel(double lat, double lon, float zoom)
        {
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

            Vector2d latlongDelta = _mapManager.WorldToGeoPosition(pos);

            return latlongDelta;
        }
    }
    
}