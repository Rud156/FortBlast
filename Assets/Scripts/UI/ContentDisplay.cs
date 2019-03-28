using UnityEngine;
using UnityEngine.UI;

namespace FortBlast.UI
{
    [RequireComponent(typeof(Text))]
    [RequireComponent(typeof(Animator))]
    public class ContentDisplay : MonoBehaviour
    {
        private const string TextAnimationParam = "FadeInOut";
        private static readonly int TextAnimation = Animator.StringToHash(TextAnimationParam);

        private Text _displayText;
        private Animator _textAnimator;

        private void Start()
        {
            _displayText = GetComponent<Text>();
            _textAnimator = GetComponent<Animator>();
        }

        public void DisplayText(string textToDisplay, Color color)
        {
            float textAlpha = _displayText.color.a;
            _displayText.color = new Color(color.r, color.g, color.b, textAlpha);

            _displayText.text = textToDisplay;
            _textAnimator.SetTrigger(TextAnimation);
        }

        #region Singleton

        public static ContentDisplay instance;

        private void Awake()
        {
            if (instance == null)
                instance = this;

            if (instance != this)
                Destroy(gameObject);
        }

        #endregion
    }
}
