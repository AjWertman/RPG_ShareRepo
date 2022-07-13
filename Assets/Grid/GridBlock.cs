using System;
using TMPro;
using UnityEngine;

public class GridBlock : MonoBehaviour
{
    public GridCoordinates gridCoordinates;
    public PathfindingCostValues pathfindingCostValues;

    public Transform travelDestination = null;

    //Refactor
    [SerializeField] Material lightMaterial = null;
    [SerializeField] Material darkMaterial = null;
    [SerializeField] Material centerMaterial = null;
    [SerializeField] Material unworthyMaterial = null;
    //

    [SerializeField] TextMeshProUGUI coordinatesText = null;

    [SerializeField] GameObject pathfindingTextContainer = null;
    [SerializeField] TextMeshProUGUI fValueText = null;
    [SerializeField] TextMeshProUGUI gValueText = null;
    [SerializeField] TextMeshProUGUI hValueText = null;

    MeshRenderer meshRenderer = null;
    
    public void InitializePiece()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();

        pathfindingTextContainer.SetActive(false);
    }

    public void SetupGridBlock(int _x, int _z)
    {
        InitializePiece();

        gridCoordinates = new GridCoordinates(_x, _z);

        SetColor(_x, _z);
        UpdateCoordinatesText(_x, _z);
    }

    public void SetColor(int _x, int _z)
    {
        if (!IsCenter())
        {
            if(IsLightBlock(_x, _z))
            {
                meshRenderer.material = lightMaterial;
                Color32 black = new Color32(0, 0, 0, 255);
                coordinatesText.color = black;
            }
            else
            {
                meshRenderer.material  = darkMaterial;
                Color32 white = new Color32(255, 255, 255, 255);
                coordinatesText.color = white;
            }
        }
        else
        {
            meshRenderer.material = centerMaterial;
            coordinatesText.color = Color.black;
        }
    }

    public void Highlight()
    {
        meshRenderer.material = centerMaterial;
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
    
    public void BecomeUnworthy()
    {
        pathfindingTextContainer.SetActive(false);
        meshRenderer.material = unworthyMaterial;
    }

    public void SetColorToWhite()
    {
        meshRenderer.material = lightMaterial;
    }

    public bool IsCenter()
    {
        Vector3 localPosition = transform.localPosition;
        bool isCenter = (localPosition.x == 0 && localPosition.z == 0);
        return isCenter;
    }

    private bool IsLightBlock(int _x, int _z)
    {
        bool isXEven = _x % 2 == 0;
        bool isZEven = _z % 2 == 0;

        if (isXEven == isZEven) return true;
        else return false;
    }
}
