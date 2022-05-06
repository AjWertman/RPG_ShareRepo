using UnityEngine;
using UnityEngine.UI;

public class TurnOrderUIItem : MonoBehaviour
{
    [SerializeField] Image background = null;
    [SerializeField] Image faceImage = null;

    int index = 0;
    BattleUnit unit = null;
    Sprite facePic = null;
    bool isPlayer = false;

    public void SetupTurnOrderUI(int _index, BattleUnit _unit)
    {
        index = _index;
        unit = _unit;
        facePic = unit.GetFaceImage();
        isPlayer = unit.IsPlayer();

        SetSize(index);
        SetImage(facePic);
        SetBackgroundColor(isPlayer);

        GetComponent<BattleUnitIndicatorTrigger>().SetupTrigger(unit);
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
        return unit;
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

    public void DecrementIndex()
    {
        index--;
        SetSize(index);
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
}
