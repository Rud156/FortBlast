using UnityEngine;
using UnityEngine.UI;

namespace FortBlast.UI
{
    public class UniSlider : MonoBehaviour
    {
        public Slider slider;

        private bool _sliderUsed;
        private GameObject _usingGameObject;

        private void Start() => slider.gameObject.SetActive(false);

        public void InitSlider(GameObject usingGameObject)
        {
            if (_sliderUsed)
                return;

            _sliderUsed = true;
            _usingGameObject = usingGameObject;
            slider.gameObject.SetActive(true);
        }

        public void UpdateSliderValue(float percent, GameObject usingGameObject)
        {
            if (_usingGameObject != usingGameObject || !_sliderUsed)
                return;

            slider.value = percent;
        }

        public void DiscardSlider(GameObject usingGameObject)
        {
            if (_usingGameObject != usingGameObject)
                return;

            _sliderUsed = false;
            _usingGameObject = null;

            slider.gameObject.SetActive(false);
        }

        #region Singleton

        public static UniSlider instance;

        private void Awake()
        {
            if (instance == null)
                instance = this;

            if (instance != this)
                Destroy(gameObject);
        }

        #endregion Singleton
    }
}