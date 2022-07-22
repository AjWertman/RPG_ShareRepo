using RPGProject.Combat;
using System;
using TMPro;
using UnityEngine;

public class GridBlock : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI coordinatesText = null;

    [SerializeField] GameObject pathfindingTextContainer = null;
    [SerializeField] TextMeshProUGUI fValueText = null;
    [SerializeField] TextMeshProUGUI gValueText = null;
    [SerializeField] TextMeshProUGUI hValueText = null;

    [SerializeField] bool isMovable = true;

    public GridCoordinates gridCoordinates;
    public PathfindingCostValues pathfindingCostValues;

    public Transform travelDestination = null;

    public Fighter contestedFighter = null;
    public Ability activeAbility = null;

    MeshRenderer meshRenderer = null;

    public event Action<Fighter, GridBlock> onContestedFighterUpdate;

    public void InitializePiece()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();

        pathfindingTextContainer.SetActive(false);
    }

    public void SetupGridBlock(Material _newMaterial, Color _textColor)
    {
        InitializePiece();
        SetColors(_newMaterial, _textColor);
        UpdateCoordinatesText(gridCoordinates.x, gridCoordinates.z);
    }

    public void SetContestedFighter(Fighter _fighter)
    {
        contestedFighter = _fighter;

        if (contestedFighter == null) return;
        onContestedFighterUpdate(contestedFighter, this);
    }

    public void SetColors(Material _newMaterial, Color _textColor)
    {
        meshRenderer.material = _newMaterial;
        coordinatesText.color = _textColor;
    }

    public void UpdateCoordinatesText(int _x, int _z)
    {
        if (!coordinatesText.gameObject.activeSelf) return;
        string coordinates = "(" + _x.ToString() + "," + _z.ToString() + ")";
        coordinatesText.text = coordinates;
    }

    public void UpdatePathfindingValueTexts(int _f, int _g, int _h)
    {
        if (!pathfindingTextContainer.activeSelf)
        {
            pathfindingTextContainer.SetActive(true);
        }

        fValueText.text = _f.ToString();
        gValueText.text = _g.ToString();
        hValueText.text = _h.ToString();
    }

    public bool IsMovable()
    {
        if (!isMovable) return false;
        if (contestedFighter != null) return false;

        return true;
    }
}
