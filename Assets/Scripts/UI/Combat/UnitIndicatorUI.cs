using UnityEngine;

namespace RPGProject.UI
{
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
