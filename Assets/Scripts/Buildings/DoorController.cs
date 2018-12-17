using FortBlast.Common;
using FortBlast.Extras;
using UnityEngine;
using UnityEngine.UI;

namespace FortBlast.Buildings
{
    public class DoorController : MonoBehaviour
    {
        [Header("Blockers")] public ParticleSystem doorParticles;
        public GameObject doorCollider;

        [Header("Time Controls")] public float deactivationTime;

        [Header("Objects")] public Transform[] switchPoints;
        public GameObject switchPrefab;

        private bool _isPlayerNearby;
        private float _currentDeactivationTime;
        private bool _gateDeactivated;

        private Slider _timeSlider;
        private Renderer _switchLightRenderer;

        private void Start() => CreateAndActivateDoorSwitch();

        private void Update()
        {
            if (_gateDeactivated)
                return;

            CheckPlayerInteractionTime();
            UpdateTimerSlider();
            CheckAndDeactivateGate();
        }

        private void OnTriggerPlayerEnter(Collider other)
        {
            if (other.CompareTag(TagManager.VisibleCollider))
                _isPlayerNearby = true;
        }

        private void OnTriggerPlayerExit(Collider other)
        {   
            if (other.CompareTag(TagManager.VisibleCollider))
                _isPlayerNearby = false;
        }

        private void CreateAndActivateDoorSwitch()
        {
            int randomPoint = Mathf.FloorToInt(Random.value * switchPoints.Length);
            GameObject switchInstance = Instantiate(switchPrefab, switchPoints[randomPoint].position,
                switchPrefab.transform.rotation);
            switchInstance.transform.SetParent(transform);

            SwitchColliderNotifier colliderNotifier = switchInstance.GetComponent<SwitchColliderNotifier>();
            colliderNotifier.triggerEnter += OnTriggerPlayerEnter;
            colliderNotifier.triggerExit += OnTriggerPlayerExit;

            _timeSlider = switchInstance.GetComponentInChildren<Slider>();
            _switchLightRenderer = switchInstance.transform.GetChild(0).GetComponent<Renderer>();
        }

        private void CheckPlayerInteractionTime()
        {
            if (Input.GetKey(Controls.InteractionKey) && _isPlayerNearby)
                _currentDeactivationTime += Time.deltaTime;
            else
                _currentDeactivationTime = 0;
        }


        private void UpdateTimerSlider() => _timeSlider.value = _currentDeactivationTime / deactivationTime;

        private void CheckAndDeactivateGate()
        {
            if (_currentDeactivationTime >= deactivationTime)
            {
                _gateDeactivated = true;

                doorCollider.SetActive(false);

                ParticleSystem.EmissionModule emission = doorParticles.emission;
                emission.rateOverTime = 0;

                _switchLightRenderer.material.SetColor("_EmissionColor", Color.red);
            }
        }
    }
}