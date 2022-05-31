using TMPro;
using UnityEngine;

public class UnitIndicatorUI : MonoBehaviour
{
    [SerializeField] GameObject playerObject = null;
    [SerializeField] GameObject enemyObject = null;

    GameObject unitObject = null;

    public void SetupUI(bool isPlayer)
    {       
        if (isPlayer)
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
