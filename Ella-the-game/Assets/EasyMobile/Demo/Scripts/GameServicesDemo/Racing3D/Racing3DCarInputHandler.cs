using System;
using System.Collections.Generic;
using UnityEngine;

namespace EasyMobile.Demo
{
    public class Racing3DCarInputHandler : MonoBehaviour
    {
        [SerializeField]
        private Racing3DCarControl hostCar = null, guestCar = null;

        [SerializeField]
        private Racing3DGameControl gameControl = null;

        [SerializeField]
        private float clickDelay = 0.4f, swipeThreshold = 20f;

        private float previousClickTime;
        private Vector2 fingerDown;
        private Vector2 fingerUp;

        protected virtual void Update()
        {
            if (!gameControl.IsPlaying)
                return;

            if(Input.touchCount > 0)
                SwipeDetect(Input.GetTouch(0));              
        }

        private void SwipeDetect(Touch touch)
        {
            if (touch.phase == TouchPhase.Began)
            {
                fingerUp = touch.position;
                fingerDown = touch.position;
            }          

            // Detects swipe after finger is released
            if (touch.phase == TouchPhase.Ended)
            {
                fingerDown = touch.position;
                if (HorizontalValMove() > swipeThreshold)
                    CheckSwipe();
            }
        }

        private float HorizontalValMove()
        {
            return Mathf.Abs(fingerDown.x - fingerUp.x);
        }

        private void CheckSwipe()
        {
            if (previousClickTime > Time.time)
            {
                previousClickTime -= Time.deltaTime;
                return;
            }

            previousClickTime = Time.time + clickDelay;
            if (fingerDown.x - fingerUp.x > 0) // Right swipe
            {               
                Move(hostCar, Racing3DGameModel.MoveDirections.Right);
                Move(guestCar, Racing3DGameModel.MoveDirections.Right);
            }             
            else if (fingerDown.x - fingerUp.x < 0) // Left swipe
            {
                Move(hostCar, Racing3DGameModel.MoveDirections.Left);
                Move(guestCar, Racing3DGameModel.MoveDirections.Left);
            }
            fingerUp = fingerDown;         
        }

        private void Move(Racing3DCarControl car, Racing3DGameModel.MoveDirections direction)
        {
            if (!car.Controllable)
                return;

            car.Move(direction);
        }
    }
}
