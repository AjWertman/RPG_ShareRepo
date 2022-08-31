using UnityEngine;

namespace RPGProject.UI
{
    /// <summary>
    /// Used to indicate a highlighted character, or if it is currently the character's turn.
    /// </summary>
    public class UnitIndicatorUI : MonoBehaviour
    {
        [SerializeField] GameObject playerObject = null;
        [SerializeField] GameObject enemyObject = null;

        GameObject unitObject = null;

        private void Start()
        {
            DeactivateIndicator();
        }

        public void ActivateIndicator(bool _isPlayer)
        {
            if (_isPlayer)
            {
                playerObject.SetActive(true);
                enemyObject.SetActive(false);
            }
            else
            {
                enemyObject.SetActive(true);
                playerObject.SetActive(false);
            }
        }

        public void DeactivateIndicator()
        {
            playerObject.gameObject.SetActive(false);
            enemyObject.gameObject.SetActive(false);
        }
    }
}
