using RPGProject.Combat;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RPGProject.UI
{
    public class TurnOrderUIItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] Image background = null;
        [SerializeField] Image faceImage = null;

        [SerializeField] Sprite defaultFaceImage = null;

        LayoutElement layoutElement = null;

        int index = 0;
        Fighter combatant = null;
        Sprite facePic = null;
        bool isPlayer = false;

        public event Action<Fighter> onPointerEnter;
        public event Action onPointerExit;

        public void InitalizeTurnOrderUIItem()
        {
            layoutElement = GetComponent<LayoutElement>();
        }

        public void SetupTurnOrderUI(int _index, Fighter _combatant)
        {
            index = _index;
            combatant = _combatant;
            facePic = combatant.characterMesh.faceImage;
            isPlayer = combatant.unitInfo.isPlayer;

            SetSize(index);
            SetImage(facePic);
            SetBackgroundColor(isPlayer);
        }

        public void ResetTurnOrderUIItem()
        {
            index = 0;
            combatant = null;
            facePic = null;
            isPlayer = false;

            SetSize(1);
            SetImage(defaultFaceImage);
            SetBackgroundColor(null);
        }

        public void SetSize(int index)
        { 
            if (index == 0)
            {
                layoutElement.preferredHeight = 75f;
                layoutElement.preferredWidth = 75f;
            }
            else
            {
                layoutElement.preferredHeight = 50f;
                layoutElement.preferredWidth = 50f;
            }
        }

        public Fighter GetCombatant()
        {
            return combatant;
        }

        public void SetImage(Sprite _facePic)
        {
            faceImage.sprite = _facePic;
        }

        public void SetBackgroundColor(bool? _isPlayer)
        {
            Color newColor = Color.white;
            if (_isPlayer == true)
            {
                newColor = new Color(0, 0, 1, .35f);
            }
            else if (_isPlayer == false)
            {
                newColor = new Color(1, 0, 0, .35f);
            }

            background.color = newColor;
        }

        public int GetIndex()
        {
            return index;
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
            onPointerEnter(combatant);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onPointerExit();
        }
    }
}
