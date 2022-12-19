using System;
using System.Collections.Generic;
using UnityEngine;

namespace EasyMobile.Demo
{
    public class Racing3DCameraControl : MonoBehaviour
    {
        [SerializeField]
        private Camera cam = null;

        [SerializeField]
        private float speed = 2;

        [SerializeField]
        private Vector3 offset;

        [SerializeField]
        private GameObject followTarget = null;

        [SerializeField]
        private bool isFollowingTarget;

        public bool IsFollowingTarget
        {
            get { return isFollowingTarget; }
            private set { isFollowingTarget = value; }
        }

        public GameObject FollowTarget
        {
            get { return followTarget; }
            private set { followTarget = value; }
        }

        public Vector3 Offset
        {
            get { return offset; }
            set { offset = value; }
        }

        public float Speed
        {
            get { return speed; }
            set
            {
                if (value < 0)
                    value = 0;

                speed = value;
            }
        }

        [ContextMenu("Log Offset")]
        public void LogOffset()
        {
            if (FollowTarget == null)
                return;

            Debug.Log("Offset: " + (FollowTarget.transform.position - cam.transform.position));
        }

        [ContextMenu("Reset Position")]
        public void ResetPosition()
        {
            cam.transform.position = new Vector3(0, 0, -10);
        }

        public void StartFollowing(GameObject followTarget)
        {
            IsFollowingTarget = true;
            FollowTarget = followTarget;
        }

        public void StopFollowing()
        {
            IsFollowingTarget = false;
            FollowTarget = null;
        }

        protected virtual void LateUpdate()
        {           
            if (!IsFollowingTarget || FollowTarget == null)
                return;

            float interpolation = speed * Time.deltaTime;
            Vector3 position = cam.transform.position;
            position.y = Mathf.Lerp(cam.transform.position.y, FollowTarget.transform.position.y + Offset.y, interpolation);
            cam.transform.position = position;
        }

        protected virtual void OnValidate()
        {
            if (speed < 0)
                speed = 0;
        }
    }
}
