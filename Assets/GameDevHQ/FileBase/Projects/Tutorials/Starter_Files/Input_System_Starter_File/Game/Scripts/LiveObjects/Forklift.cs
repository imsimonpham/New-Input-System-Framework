using System;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

namespace Game.Scripts.LiveObjects
{
    public class Forklift : MonoBehaviour
    {
        [SerializeField]
        private GameObject _lift, _steeringWheel, _leftWheel, _rightWheel, _rearWheels;
        [SerializeField]
        private Vector3 _liftLowerLimit, _liftUpperLimit;
        [SerializeField]
        private float _speed = 5f, _liftSpeed = 1f;
        [SerializeField]
        private CinemachineVirtualCamera _forkliftCam;
        [SerializeField]
        private GameObject _driverModel;
        private bool _inDriveMode = false;
        [SerializeField]
        private InteractableZone _interactableZone;

        public static event Action onDriveModeEntered;
        public static event Action onDriveModeExited;

        private PlayerInputActions _input;

        private void OnEnable()
        {
            InteractableZone.onZoneInteractionComplete += EnterDriveMode;
        }

        //New Input System
        private void Start()
        {
            _input = new PlayerInputActions();
            _input.Forklift.Enable();
            _input.Forklift.Exit.performed += Exit_performed;
        }

        private void Exit_performed(InputAction.CallbackContext context)
        {
            if (_inDriveMode == true)
            {
                ExitDriveMode();
            }
        }

        private void EnterDriveMode(InteractableZone zone)
        {
            if (_inDriveMode !=true && zone.GetZoneID() == 5) //Enter ForkLift
            {
                _inDriveMode = true;
                _forkliftCam.Priority = 11;
                onDriveModeEntered?.Invoke();
                _driverModel.SetActive(true);
                _interactableZone.CompleteTask(5);
            }
        }

        private void ExitDriveMode()
        {
            _inDriveMode = false;
            _forkliftCam.Priority = 9;            
            _driverModel.SetActive(false);
            onDriveModeExited?.Invoke();
            
        }

        private void Update()
        {
            if (_inDriveMode == true)
            {
                if(_lift.transform.localPosition.y >= _liftUpperLimit.y)
                    _lift.transform.localPosition = _liftUpperLimit;
                else if(_lift.transform.localPosition.y <= _liftLowerLimit.y)
                    _lift.transform.localPosition = _liftLowerLimit;
                LiftControls();
                CalcutateMovement();
                /*if (Input.GetKeyDown(KeyCode.Escape))
                    ExitDriveMode();*/
            }

        }

        private void CalcutateMovement()
        {
            //Old Input system
            /*float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            var direction = new Vector3(0, 0, v);

            var velocity = direction * _speed;

            transform.Translate(velocity * Time.deltaTime);

            if (Mathf.Abs(v) > 0)
            {
                var tempRot = transform.rotation.eulerAngles;
                tempRot.y += h * _speed / 2;
                transform.rotation = Quaternion.Euler(tempRot);
            }*/

            //New input system
            var move = _input.Forklift.Movement.ReadValue<Vector2>();
            transform.Translate(new Vector3(0, 0, move.y) * Time.deltaTime * _speed);

            if (Mathf.Abs(move.x) > 0)
            {
                var tempRot = transform.rotation.eulerAngles;
                tempRot.y += move.x * _speed / 2;
                transform.rotation = Quaternion.Euler(tempRot);
            }
            
        }

        private void LiftControls()
        {
            //Old Input System
            /*if (Input.GetKey(KeyCode.R))
                LiftUpRoutine();
            else if (Input.GetKey(KeyCode.T))
                LiftDownRoutine();*/

            //New Input System
            var lift = _input.Forklift.Lift.ReadValue<float>();
            Vector3 tempPos = _lift.transform.localPosition;
            tempPos.y += Time.deltaTime * _liftSpeed * lift;
            _lift.transform.localPosition = new Vector3(tempPos.x, tempPos.y, tempPos.z);
        }

        /*private void LiftUpRoutine()
        {
            if (_lift.transform.localPosition.y < _liftUpperLimit.y)
            {
                Vector3 tempPos = _lift.transform.localPosition;
                tempPos.y += Time.deltaTime * _liftSpeed;
                _lift.transform.localPosition = new Vector3(tempPos.x, tempPos.y, tempPos.z);
            }
            else if (_lift.transform.localPosition.y >= _liftUpperLimit.y)
                _lift.transform.localPosition = _liftUpperLimit;
        }

        private void LiftDownRoutine()
        {
            if (_lift.transform.localPosition.y > _liftLowerLimit.y)
            {
                Vector3 tempPos = _lift.transform.localPosition;
                tempPos.y -= Time.deltaTime * _liftSpeed;
                _lift.transform.localPosition = new Vector3(tempPos.x, tempPos.y, tempPos.z);
            }
            else if (_lift.transform.localPosition.y <= _liftLowerLimit.y)
                _lift.transform.localPosition = _liftLowerLimit;
        }*/

        private void OnDisable()
        {
            InteractableZone.onZoneInteractionComplete -= EnterDriveMode;
        }

    }
}