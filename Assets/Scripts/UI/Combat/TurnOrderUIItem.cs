using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TurnOrderUIItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image background = null;
    [SerializeField] Image faceImage = null;

    int index = 0;
    BattleUnit battleUnit = null;
    Sprite facePic = null;
    bool isPlayer = false;

    public event Action<BattleUnit> onPointerEnter;
    public event Action onPointerExit;

    public void SetupTurnOrderUI(int _index, BattleUnit _battleUnit)
    {
        index = _index;
        battleUnit = _battleUnit;
        facePic = battleUnit.GetBattleUnitInfo().GetFaceImage();
        isPlayer = battleUnit.GetBattleUnitInfo().IsPlayer();

        SetSize(index);
        SetImage(facePic);
        SetBackgroundColor(isPlayer);
    }

    public void SetSize(int index)
    {
        LayoutElement layoutElement = GetComponent<LayoutElement>();
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

    public void SetBackgroundColor(bool isPlayer)
    {       
        if (isPlayer)
        {
            Color playerColor = new Color(0, 0, 1, .35f);
            background.color = playerColor;
        }
        else
        {
            Color enemyColor = new Color(1, 0, 0, .35f);
            background.color = enemyColor;
        }
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
