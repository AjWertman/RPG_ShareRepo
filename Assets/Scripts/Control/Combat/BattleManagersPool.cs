using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Control.Combat
{
    public class BattleManagersPool : MonoBehaviour
    {
        [SerializeField] GameObject battlePositionManagerPrefab = null;
        [SerializeField] GameObject unitManagerPrefab = null;
        [SerializeField] GameObject battleUIManagerPrefab = null;
        [SerializeField] GameObject turnManagerPrefab = null;

        [SerializeField] GameObject battleCamGameObject= null;

        BattleGridManager battleGridManager = null;
        UnitManager unitManager = null;
        BattleUIManager battleUIManager = null;
        TurnManager turnManager = null;

        Transform startingParent = null;

        private void Start()
        {
            startingParent = transform.parent;
            CreateManagers();
        }

        private void CreateManagers()
        {
            GameObject gridInstance = Instantiate(battlePositionManagerPrefab, transform);
            battleGridManager = gridInstance.GetComponent<BattleGridManager>();

            GameObject unitInstance = Instantiate(unitManagerPrefab, transform);
            unitManager = unitInstance.GetComponent<UnitManager>();
            unitManager.InitalizeUnitManager();

            GameObject uiInstance = Instantiate(battleUIManagerPrefab, transform);
            battleUIManager = uiInstance.GetComponent<BattleUIManager>();
            battleUIManager.InitalizeBattleUIManager();

            GameObject turnInstance = Instantiate(turnManagerPrefab, transform);
            turnManager = turnInstance.GetComponent<TurnManager>();

            ActivateManagersGameObjects(false);
        }

        public void ActivateManagersPool()
        {
            ActivateManagersGameObjects(true);
        }

        public void ResetManagersPool()
        {
            ResetManagers();

            battleCamGameObject.transform.parent = transform;
            battleCamGameObject.transform.localPosition = Vector3.zero;
            battleCamGameObject.transform.localEulerAngles = Vector3.zero;

            transform.parent = startingParent;
            transform.position = Vector3.zero;
            transform.eulerAngles = Vector3.zero;
            ActivateManagersGameObjects(false);
        }

        private void ResetManagers()
        {
            unitManager.ResetUnitManager();
            battleUIManager.ResetUIManager();
            turnManager.ResetTurnManager();
        }

        private void ActivateManagersGameObjects(bool _shouldActivate)
        {
            foreach (GameObject gameObject in GetAllManagers())
            {
                gameObject.SetActive(_shouldActivate);
            }
        }

        public BattleGridManager GetBattlePositionManager()
        {
            return battleGridManager;
        }

        public UnitManager GetUnitManager()
        {
            return unitManager;
        }

        public BattleUIManager GetBattleUIManager()
        {
            return battleUIManager;
        }

        public TurnManager GetTurnManager()
        {
            return turnManager;
        }

        public GameObject GetBattleCamInstance()
        {
            return battleCamGameObject;
        }

        public IEnumerable<GameObject> GetAllManagers()
        {
            yield return battleGridManager.gameObject;
            yield return unitManager.gameObject;
            yield return battleUIManager.gameObject;
            yield return turnManager.gameObject;

            yield return battleCamGameObject;
        }
    }
}
