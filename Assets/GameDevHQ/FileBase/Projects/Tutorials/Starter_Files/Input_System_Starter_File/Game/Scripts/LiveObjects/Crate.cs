using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Scripts.LiveObjects
{
    public class Crate : MonoBehaviour
    {
        [SerializeField] private float _punchDelay;
        [SerializeField] private GameObject _wholeCrate, _brokenCrate;
        [SerializeField] private Rigidbody[] _pieces;
        [SerializeField] private BoxCollider _crateCollider;
        [SerializeField] private InteractableZone _interactableZone;
        private bool _isReadyToBreak = false;

        private List<Rigidbody> _brakeOff = new List<Rigidbody>();

        //New Input System
        private float _timeToBreakOnePiece = 0.5f;
        private int _pieceCount;
        private PlayerInputActions _input;

        private bool _canBreak = false;


        private void Start()
        {
            _brakeOff.AddRange(_pieces);

            _input = new PlayerInputActions();
            _input.Crate.Enable();
            _input.Crate.Break.canceled += Break_canceled;
        }

        private void Break_canceled(InputAction.CallbackContext context)
        {
            if (_canBreak)
            {
                float holdTime = (float)context.duration;
                if (holdTime < 1f)
                {
                    _pieceCount = 1;
                }
                else if (holdTime < 2f)
                {
                    _pieceCount = 3;
                    
                } else
                {
                    _pieceCount = 5;
                }

                if (_pieceCount <= _brakeOff.Count)
                {
                    for (int i = 0; i < _pieceCount; i++)
                    {
                        BreakPart();
                        StartCoroutine(PunchDelay());
                    }
                }
                else
                {
                    for (int i = 0; i < _brakeOff.Count; i++)
                    {
                        BreakPart();
                        StartCoroutine(PunchDelay());
                    }
                }
            } 
        }

        private void OnEnable()
        {
            InteractableZone.onZoneInteractionComplete += InteractableZone_onZoneInteractionComplete;
        }


        private void InteractableZone_onZoneInteractionComplete(InteractableZone zone)
        {

            if (_isReadyToBreak == false && _brakeOff.Count > 0)
            {
                _wholeCrate.SetActive(false);
                _brokenCrate.SetActive(true);
                _isReadyToBreak = true;
            }

            if (_isReadyToBreak && zone.GetZoneID() == 6) //Crate zone            
            {
                if (_brakeOff.Count > 0)
                {
                    /*BreakPart();
                    StartCoroutine(PunchDelay());*/
                    _canBreak = true;
                }
                else if (_brakeOff.Count == 0)
                {
                    _canBreak = false;
                    _isReadyToBreak = false;
                    _crateCollider.enabled = false;
                    _interactableZone.CompleteTask(6);
                    Debug.Log("Completely Busted");
                }
            }
        }

        public void BreakPart()
        {
            int rng = Random.Range(0, _brakeOff.Count);
            _brakeOff[rng].constraints = RigidbodyConstraints.None;
            _brakeOff[rng].AddForce(new Vector3(1f, 1f, 1f), ForceMode.Force);
            _brakeOff.Remove(_brakeOff[rng]);
            Debug.Log("Broke");
        }

        IEnumerator PunchDelay()
        {
            float delayTimer = 0;
            while (delayTimer < _punchDelay)
            {
                yield return new WaitForEndOfFrame();
                delayTimer += Time.deltaTime;
            }

            _interactableZone.ResetAction(6);
        }

        private void OnDisable()
        {
            InteractableZone.onZoneInteractionComplete -= InteractableZone_onZoneInteractionComplete;
        }
    }
}
