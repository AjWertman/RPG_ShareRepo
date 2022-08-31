using RPGProject.Combat;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RPGProject.UI
{
    /// <summary>
    /// The object instance of UI that is represented by the turn order of battle.
    /// </summary>
    public class TurnOrderUIItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] Image background = null;
        [SerializeField] Image faceImage = null;

        [SerializeField] Sprite defaultFaceImage = null;

        LayoutElement layoutElement = null;

        Fighter combatant = null;
        Sprite facePic = null;
        bool isPlayer = false;

        public event Action<Fighter> onHighlight;
        public event Action onUnhighlight;

        public void InitalizeTurnOrderUIItem()
        {
            layoutElement = GetComponent<LayoutElement>();
        }

        public void SetupTurnOrderUI(Fighter _combatant)
        {
            combatant = _combatant;
            CharacterMesh characterMesh = combatant.characterMesh;
            facePic = characterMesh.faceImage;

            SetImage(facePic);
            background.color = characterMesh.uiColor; 
        }

        public void ResetTurnOrderUIItem()
        {
            combatant = null;
            facePic = null;
            isPlayer = false;

            SetImage(defaultFaceImage);
            background.color = Color.white ;
        }

        public Fighter GetCombatant()
        {
            return combatant;
        }

        public void SetImage(Sprite _facePic)
        {
            faceImage.sprite = _facePic;
        }

        public Sprite GetFaceImage()
        {
            return facePic;
        }

        public bool IsPlayer()
        {
            return isPlayer;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (combatant == null) return;
            onHighlight(combatant);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onUnhighlight();
        }
    }
}
