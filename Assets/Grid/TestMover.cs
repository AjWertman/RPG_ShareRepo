using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestMover : MonoBehaviour
{
    List<GridBlock> path = new List<GridBlock>();

    Pathfinder pathfinder = null;
    NavMeshAgent navMeshAgent = null;

    GridBlock currentHighlightedBlock = null;

    private void Awake()
    {
        pathfinder = FindObjectOfType<Pathfinder>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {

            GridBlock gridBlock = hit.collider.GetComponentInParent<GridBlock>();

            if (gridBlock == null) return;
            if (gridBlock == currentHighlightedBlock) return;
            ClearPath();
            GridCoordinates zero = new GridCoordinates(0, 0);

            path = pathfinder.FindPath(zero, gridBlock.gridCoordinates);

            for (int i = 0; i < path.Count; i++)
            {
                string coordinatesString = ("(" + path[i].gridCoordinates.x.ToString() + "," + path[i].gridCoordinates.z.ToString() + ")");
                print(i.ToString() + " - " + coordinatesString);

                path[i].Highlight();
            }

            //foreach(GridBlock thisBlock in pathfinder.GetNeighbors(gridBlock.gridCoordinates))
            //{
            //    if (thisBlock == null) continue;
            //    thisBlock.BecomeUnworthy();
            //}
            //Vector3 piecePosition = gridBlock.travelDestination.position;
            //navMeshAgent.SetDestination(piecePosition);
        }
    }

    private void ClearPath()
    {
       foreach(GridBlock gridBlock in path)
        {
            GridCoordinates gridCoordinates = gridBlock.gridCoordinates;
            gridBlock.SetColor(gridCoordinates.x, gridCoordinates.z);
        }
    }
}
