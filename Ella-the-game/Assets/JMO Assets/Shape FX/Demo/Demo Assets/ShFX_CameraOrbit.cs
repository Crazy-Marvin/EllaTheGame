//========================================
//
// SHAPE FX
// (c) 2016 - Jean Moreno
// http://www.jeanmoreno.com/unity
//
//========================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ShapeFX
{
	namespace Demo
	{
		public class ShFX_CameraOrbit : MonoBehaviour
		{
			//--------------------------------------------------------------------------------------------------
			// PUBLIC INSPECTOR PROPERTIES
			
			public Camera Cam;
			public Vector3 PivotPosition;
			[Header("Orbit")]
			public float OrbitStrg = 3f;
			public float OrbitClamp = 50f;
			[Header("Zooming")]
			public float ZoomStrg = 40f;
			public float ZoomClamp = 30f;
			public float ZoomDist = 50f;
			[Header("Misc")]
			public float Decceleration = 8f;

			//--------------------------------------------------------------------------------------------------
			// PRIVATE PROPERTIES
			
			private Vector3 mouseDelta;
			private Vector3 orbitAcceleration;
			private Vector3 moveAcceleration;
			private float zoomAcceleration;
			private const float XMax = 60;
			private const float XMin = 300;

			private Vector3 mResetCamPos, mResetCamRot;

			private float DeltaTime { get { return Time.deltaTime/Time.timeScale; } }

			//--------------------------------------------------------------------------------------------------
			// UNITY EVENTS

			void Awake()
			{
				mResetCamPos = Cam.transform.position;
				mResetCamRot = Cam.transform.eulerAngles;
			}

			void OnEnable()
			{
				mouseDelta = Input.mousePosition;
			}

			void Update()
			{
				mouseDelta = Input.mousePosition - mouseDelta;

				//Left Button held
				if(Input.GetMouseButton(0))
				{
					orbitAcceleration.x += Mathf.Clamp(mouseDelta.x * OrbitStrg, -OrbitClamp, OrbitClamp);
					orbitAcceleration.y += Mathf.Clamp(-mouseDelta.y * OrbitStrg, -OrbitClamp, OrbitClamp);
				}

				if(Input.GetKeyDown(KeyCode.R))
				{
					ResetView();
				}

				//X Angle Clamping
				Vector3 angle = Cam.transform.localEulerAngles;
				if(angle.x < 180 && angle.x >= XMax && orbitAcceleration.y > 0) orbitAcceleration.y = 0;
				if(angle.x > 180 && angle.x <= XMin && orbitAcceleration.y < 0) orbitAcceleration.y = 0;

				//Rotate
				Cam.transform.RotateAround(PivotPosition, Cam.transform.right, orbitAcceleration.y * DeltaTime);
				Cam.transform.RotateAround(PivotPosition, Vector3.up, orbitAcceleration.x * DeltaTime);
				//Zoom
				float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
				zoomAcceleration += scrollWheel * ZoomStrg;
				zoomAcceleration = Mathf.Clamp(zoomAcceleration, -ZoomClamp, ZoomClamp);
				float dist = Vector3.Distance(Cam.transform.position, PivotPosition);
				if((dist >= 1 && zoomAcceleration > 0) || (dist <= ZoomDist && zoomAcceleration < 0))
				{
					Cam.transform.Translate(Vector3.forward * zoomAcceleration * DeltaTime, Space.Self);
				}

				//Deccelerate
				orbitAcceleration = Vector3.Lerp(orbitAcceleration, Vector3.zero, Decceleration * DeltaTime);
				zoomAcceleration = Mathf.Lerp(zoomAcceleration, 0, Decceleration * DeltaTime);
				moveAcceleration = Vector3.Lerp(moveAcceleration, Vector3.zero, Decceleration * DeltaTime);

				mouseDelta = Input.mousePosition;
			}

			public void ResetView()
			{
				Cam.transform.position = mResetCamPos;
				Cam.transform.eulerAngles = mResetCamRot;
			}
		}
	}
}