using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RPGProject.Combat
{
    public class TurnOrderUIItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] Image background = null;
        [SerializeField] Image faceImage = null;

        [SerializeField] Sprite defaultFaceImage = null;

        LayoutElement layoutElement = null;

        int index = 0;
        BattleUnit battleUnit = null;
        Sprite facePic = null;
        bool isPlayer = false;

        public event Action<BattleUnit> onPointerEnter;
        public event Action onPointerExit;

        public void InitalizeTurnOrderUIItem()
        {
            layoutElement = GetComponent<LayoutElement>();
        }

        public void SetupTurnOrderUI(int _index, BattleUnit _battleUnit)
        {
            index = _index;
            battleUnit = _battleUnit;
            facePic = battleUnit.GetCharacterMesh().GetFaceImage();
            isPlayer = battleUnit.GetBattleUnitInfo().IsPlayer();

            SetSize(index);
            SetImage(facePic);
            SetBackgroundColor(isPlayer);
        }

        public void ResetTurnOrderUIItem()
        {
            index = 0;
            battleUnit = null;
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

        public BattleUnit GetBattleUnit()
        {
            return battleUnit;
        }

        public void SetImage(Sprite facePic)
        {
            faceImage.sprite = facePic;
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
            if (battleUnit == null) return;
            onPointerEnter(battleUnit);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onPointerExit();
        }
    }
}
