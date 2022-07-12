using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestMover : MonoBehaviour
{
    NavMeshAgent navMeshAgent = null;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit))
            {
                GridPiece gridPiece = hit.collider.GetComponentInParent<GridPiece>();

                Vector3 piecePosition = gridPiece.GetDestination().position;
                navMeshAgent.SetDestination(piecePosition);
            }
        }
    }
}
