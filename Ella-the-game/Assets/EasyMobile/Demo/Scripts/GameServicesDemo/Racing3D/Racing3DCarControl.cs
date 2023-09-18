using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace EasyMobile.Demo
{
    public class Racing3DCarControl : MonoBehaviour
    {
        [Serializable]
        public struct OverheadInfo
        {
            public RawImage avatarImage;
            public Text nameText;

            public void Update(Texture2D avatar, string name)
            {
                UpdateAvatar(avatar);
                UpdateName(name);
            }

            public void UpdateName(string name)
            {
                nameText.text = name;
            }

            public void UpdateAvatar(Texture2D texture)
            {
                avatarImage.texture = texture;
            }
        }

        [SerializeField]
        private Rigidbody rb = null;

        [SerializeField]
        private GameObject hightlightObject = null;

        [SerializeField]
        private Racing3DGameControl gameControl = null;

        [SerializeField]
        private Racing3DGameView gameView = null;

        [Space]
        [SerializeField]
        private bool controlable;

        [SerializeField, Range(0, 50)]
        private int nitro = 0;

        [SerializeField]
        private string powerUpsName = "Nitro", finishLineName = "FinishLine";

        [SerializeField]
        private float defaultSpeed = 5f,
            maxSpeed = 50f,
            nitroBoost = 0.075f,
            nitroBoostFrame = 0.25f,
            nitroAutoFillRate = 0.5f,
            moveDistance = 0.5f;

        [SerializeField]
        private Vector3 defaultPosition = Vector3.zero;

        [SerializeField]
        private int defaultRoadLine = 0,
            maxRoadLine = 3,
            finishLineSlowdownFrames = 120;

        [SerializeField]
        private Vector3 prepareCameraPosition = Vector3.zero;

        [SerializeField]
        private OverheadInfo info = default(OverheadInfo);

        public bool IsMoving { get; private set; }

        public bool IsUsingNitro { get; private set; }

        public float RunDistance { get; private set; }

        public bool Controllable
        {
            get { return controlable; }
            set
            {
                controlable = value;
                hightlightObject.SetActive(value);
            }
        }

        public float Speed
        {
            get { return speed; }
            set { speed = Mathf.Clamp(value, 0, maxSpeed); }
        }

        public int Nitro
        {
            get { return nitro; }
            set { nitro = Mathf.Clamp(value, 0, 50); }
        }

        public int CurrentRoadLine { get; private set; }

        public Vector3 PrepareCameraPosition { get { return prepareCameraPosition; } }

        private Coroutine useNitroCoroutine = null;
        private float speed;

        protected virtual void OnValidate()
        {
            if (speed < 0)
                speed = 0;

            if (maxSpeed < speed)
                maxSpeed = speed;

            if (nitroBoost < 0)
                nitroBoost = 0;

            if (nitroAutoFillRate < 0.01f)
                nitroAutoFillRate = 0.01f;

            if (nitroBoostFrame < 0)
                nitroBoostFrame = 0;

            if (moveDistance < 0)
                moveDistance = 0;
        }


        protected virtual void Start()
        {
            hightlightObject.SetActive(Controllable);
        }

        protected virtual void FixedUpdate()
        {
            if (!IsMoving)
                return;

            // Auto move forward.
            rb.velocity = new Vector3(rb.velocity.x, Speed);
            RunDistance += Speed;
        }

        public void ResetValues()
        {
            IsMoving = false;
            transform.localPosition = defaultPosition;
            Nitro = 50;
            Speed = defaultSpeed;
            CurrentRoadLine = defaultRoadLine;
            RunDistance = 0f;
            StopAllCoroutines();
        }

        public void StartMoving(bool controllable)
        {
            if (IsMoving)
                return;

            Controllable = controllable;
            IsMoving = true;
        }

        public void ContinueMoving()
        {
            if (IsMoving)
                return;

            IsMoving = true;
        }

        public void StopMoving(int slowdownFrames = 0)
        {
            if (!IsMoving)
                return;

            if (slowdownFrames <= 0)
            {
                rb.velocity = Vector3.zero;
                IsMoving = false;
            }
            else
            {
                StartCoroutine(SlowDownCoroutine(slowdownFrames));
            }

            if (IsUsingNitro && useNitroCoroutine != null)
                StopCoroutine(useNitroCoroutine);

        }

        public void Move(Racing3DGameModel.MoveDirections direction)
        {
            if ((direction == Racing3DGameModel.MoveDirections.Left && CurrentRoadLine < 1) ||
                (direction == Racing3DGameModel.MoveDirections.Right && CurrentRoadLine >= maxRoadLine))
                return;

            int newRoadLine = direction == Racing3DGameModel.MoveDirections.Left ? CurrentRoadLine - 1 : CurrentRoadLine + 1;

            if (gameControl.OpponentCar.CurrentRoadLine == newRoadLine)
            {
                if (Mathf.Abs(transform.position.y - gameControl.OpponentCar.transform.position.y) <= 0.9f)
                    return;
            }


            if (direction == Racing3DGameModel.MoveDirections.Left)
            {
                CurrentRoadLine--;
                rb.position -= new Vector3(moveDistance, 0f, 0f);
            }
            else
            {
                CurrentRoadLine++;
                rb.position += new Vector3(moveDistance, 0f, 0f);
            }

            if (Controllable)
                gameControl.SendMoveMessage(direction);
        }

        public void UseNitro()
        {
            Nitro = 100;
            if (nitro <= 0 || IsUsingNitro || !IsMoving)
                return;

            useNitroCoroutine = StartCoroutine(UseNitroCoroutine());

            if (Controllable)
                gameControl.SendUseNitroMessage();
        }

        public void CreateHitPowerUpEffect()
        {
            //Debug.Log("Hit power up!!!");
        }

        private IEnumerator UseNitroCoroutine()
        {
            if (!IsMoving)
                yield break;

            IsUsingNitro = true;
            int steps = 0, maxSteps = nitro;
            float originalSpeed = speed;
            float maxSpeed = originalSpeed + maxSteps * nitroBoost;

            while (steps <= maxSteps)
            {
                speed = Mathf.Lerp(originalSpeed, maxSpeed, (float)steps / maxSteps);
                steps++;
                nitro--;
                yield return null;
            }

            speed = maxSpeed;
            IsUsingNitro = false;
            gameView.SetActiveNitro(false);
        }

        public void UpdateInfo(Texture2D avatar, string name)
        {
            info.Update(avatar, name);
        }

        public void UpdateName(string name)
        {
            info.UpdateName(name);
        }

        public void UpdateAvatar(Texture2D avatar)
        {
            info.UpdateAvatar(avatar);
        }

        private IEnumerator SlowDownCoroutine(int slowdownFrames)
        {
            if (slowdownFrames < 0)
                yield break;

            var factor = Speed / slowdownFrames;
            int currentFrame = 0;
            while (currentFrame <= slowdownFrames)
            {
                Speed -= factor;
                yield return new WaitForEndOfFrame();
            }
            Speed = 0;
            IsMoving = false;
            rb.velocity = Vector3.zero;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!Controllable)
            {
                if (other.gameObject.name.Equals(powerUpsName))
                {
                    other.gameObject.SetActive(false);
                }
            }

            if (!Controllable || !gameControl.IsPlaying)
                return;

            if (other.gameObject.name.Equals(finishLineName))
            {
                StopMoving(finishLineSlowdownFrames);
                gameControl.SendFinishMessage();
                gameControl.IsPlaying = false;
                gameView.ShowGameOverUI(true, Racing3DGameModel.GameoverReason.FinishRace);
                gameControl.OpponentCar.StopMoving();
                return;
            }

            if (other.gameObject.name.Equals(powerUpsName))
            {
                /*
                StopMoving();
                gameControl.SendHitObstacleMessage();
                gameControl.IsPlaying = false;
                
                gameView.ShowGameOverUI(false, Racing3DGameModel.GameoverReason.HitObstacle);
                gameControl.OpponentCar.StopMoving();
                */
                CreateHitPowerUpEffect();
                gameView.SetActiveNitro(true);
                UseNitro();
                other.gameObject.SetActive(false);
                return;
            }
        }
    }
}
