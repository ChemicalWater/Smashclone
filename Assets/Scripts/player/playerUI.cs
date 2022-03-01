using UnityEngine;
using UnityEngine.UI;
using TMPro;


using System.Collections;


namespace smashclone
{
    public class playerUI : MonoBehaviour
    {
        #region Private Fields
        private playerControl target;

        [Tooltip("UI Text to display Player 1's Name")]
        [SerializeField]
        private TextMeshProUGUI player1Name;

        [Tooltip("UI Text to display Player 2's Name")]
        [SerializeField]
        private TextMeshProUGUI player2Name;


        [Tooltip("UI Slider to display Player 1's Health")]
        [SerializeField]
        private Image player1HealthSlider;

        [Tooltip("UI Slider to display Player 2's Health")]
        [SerializeField]
        private Image player2HealthSlider;


        #endregion


        #region MonoBehaviour Callbacks

        void Update()
        {
            // Reflect the Player Health
            if (player1HealthSlider != null)
            {
                player1HealthSlider.fillAmount = target.health;
            }

            if (target == null)
            {
                Destroy(this.gameObject);
                return;
            }

        }

        #endregion


        #region Public Methods

        public void SetTarget(playerControl _target)
        {
            if (_target == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> PlayMakerManager target for PlayerUI.SetTarget.", this);
                return;
            }
            // Cache references for efficiency
            target = _target;
            if (player1Name != null)
            {
                player1Name.text = target.photonView.Owner.NickName;
            }
        }

        #endregion


    }
}