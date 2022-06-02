using UnityEngine;

namespace RPGProject.Combat
{
    public class UnitIndicatorUI : MonoBehaviour
    {
        //Refactor - class/character (charMesh) specific indicators? -
        [SerializeField] GameObject playerObject = null;
        [SerializeField] GameObject enemyObject = null;

        GameObject unitObject = null;

        public void SetupUI(bool _isPlayer)
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
    }
}
