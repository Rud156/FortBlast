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
        public Slider timeSlider;

        [Header("Objects")] public Transform[] switchPoints;
        public GameObject switchPrefab;

        private bool _isPlayerNearby;
        private float _currentDeactivationTime;
        private bool _gateDeactivated;

        private void Update()
        {
            if (_gateDeactivated)
                return;

            CheckPlayerInteractionTime();
            UpdateTimerSlider();
            CheckAndDeactivateGate();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(TagManager.Player))
                _isPlayerNearby = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(TagManager.Player))
                _isPlayerNearby = false;
        }

        private void CheckPlayerInteractionTime()
        {
            if (Input.GetKey(Controls.InteractionKey) && _isPlayerNearby)
                _currentDeactivationTime += Time.deltaTime;
            else
                _currentDeactivationTime = 0;
        }


        private void UpdateTimerSlider() => timeSlider.value = _currentDeactivationTime / deactivationTime;

        private void CheckAndDeactivateGate()
        {
            if (_currentDeactivationTime >= deactivationTime)
            {
                _gateDeactivated = true;

                doorCollider.SetActive(false);

                ParticleSystem.EmissionModule emission = doorParticles.emission;
                emission.rateOverTime = 0;
            }
        }
    }
}