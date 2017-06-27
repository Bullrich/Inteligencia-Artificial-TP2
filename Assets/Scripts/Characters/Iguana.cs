﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Blue.Pathfinding;

public class Iguana : MonoBehaviour
{


    enum Input
    {
        TargetSeen,
        TargetNotSeen,
        FollowNoise,
        ReachedDestination
    }

    public GameObject[] helmetPrefabs;
    public Transform helmetPos;
    public float[] rankProbability;
    public int rank;
    public float _maxSpeed;

    int _iguanaIndex;
    Color _gizmoColor;

    LineOfSight _los;
    CharacterController _controller;
    Animator _animator;

    private Input currentState;

    Vector3[] path;
    public int targetIndex = 0;
    public Vector3 currentTarget;

    Vector3 _startPatrol, _endPatrol;
    bool _hasRequestedPath = false,
    patrollingInversed;



    protected void Start()
    {
        Debug.Assert(helmetPrefabs.Length == rankProbability.Length);
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _los = GetComponent<LineOfSight>();
        currentTarget = transform.position;
        Invoke("StartNow", 1f);
    }

    private void StartNow()
    {
        Vector3 targetPosition = new Vector3(15.35f, 6.3f, 12f);
        GetPatrolRoute(targetPosition);
    }

    void ProcessInput(Input input)
    {

    }



    void CreatePatrolPath(MapNode startNode)
    {

    }

    int CalculateRandomRank()
    {
        return 0;
    }

    public void Configure(int iguanaIndex, Color gizmoColor, MapNode startNode)
    {
        _iguanaIndex = iguanaIndex;
        _gizmoColor = gizmoColor;

        //ALUM: Calcular rango por aleatoriedad con pesos
        var rank = CalculateRandomRank();

        name = "Iguana " + iguanaIndex + " Rank " + rank;       //Le ponemos nombre
        _maxSpeed = 2f + rank * 2.5f;                           //Damos distintas velocidades por rango militar

        //ALUM: Crear el casco en "helmetPos" y armar el recorrido de patrullaje

    }

    void MoveToPoint(Vector3 targetPosition)
    {
        if (targetPosition == transform.position)
            return;

        Vector3 moveDiff = targetPosition - transform.position;
        Vector3 moveDir = moveDiff.normalized * _maxSpeed * Time.deltaTime;
        if (moveDir.sqrMagnitude < moveDiff.sqrMagnitude)
        {
            _controller.Move(moveDir);
        }
        else
        {
            _controller.Move(moveDiff);
        }
    }

    void Scream()
    {
        _animator.SetTrigger("Hit");
        //ALUM: Generar "ruido"

    }

    private void GetPatrolRoute(Vector3 targetPosition)
    {
        PathRequestManager.RequestPath(transform.position, targetPosition, OnPathFound);
        _hasRequestedPath = true;
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            targetIndex = 0;
            // Move to path
            currentState = Input.TargetNotSeen;
            // foreach (Vector3 vec in newPath)
            //     print(vec);
        }
        _hasRequestedPath = false;
    }

    // Update is called once per frame
    protected void Update()
    {
        //ALUM: Alimentar la FSM con sensores



        //ALUM: Moverse acorde al estado.
        Vector3 delta = Vector3.zero;
        switch (currentState)
        {
            case Input.TargetNotSeen:
                Patrolling();
                //delta = Vector3.one;
                break;
        }


        if (delta.sqrMagnitude > 0.1f)
        {
            var motionMagnitude = _maxSpeed;

            //ALUM: Truncar la velocidad a una velocidad máxima y animar con la velocidad real (en vez de _maxSpeed)
            transform.forward = currentTarget;
            _controller.Move(Vector3.forward * _maxSpeed);

            _animator.SetFloat("Forward", motionMagnitude);
        }
        else
        {
            _animator.SetFloat("Forward", 0f);          //Frenar animación
            _controller.SimpleMove(Vector3.zero);       //Aplicar gravedad
        }

    }

    private void Patrolling()
    {
        if (targetIndex >= path.Length || targetIndex < 0)
        {
            patrollingInversed = !patrollingInversed;
            targetIndex += (patrollingInversed ? -1 : 1);
        }
        else if (Vector3.Distance(path[targetIndex], transform.position) > 1)
        {
            var offset = path[targetIndex] - transform.position;
            if (offset.magnitude > .1f)
            {
                currentTarget = path[targetIndex];
                MoveToPoint(currentTarget);
                transform.LookAt(currentTarget);
            }
        }
        else
            targetIndex += (patrollingInversed ? -1 : 1);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = _gizmoColor;                     //Color único por iguana
        var offset = Vector3.one * 0.15f * _iguanaIndex;    //Un pequeño offset para que no este todo superpuesto (usando el índice de la iguana)

        Gizmos.DrawSphere(transform.position + offset, 0.5f);   //Esfera sobre la iguana para saber el color asociado a la misma

        //ALUM: Dibujar la ruta de patrullaje o la que se está siguiendo de momento. Utilizar "offset" para no superponer los gizmos.

        if (path != null)
        {
            for (int i = 0; i < path.Length; i++)
            {
                Gizmos.color = (i ==targetIndex ? Color.red:Color.black);
                Gizmos.DrawCube(path[i], Vector3.one);

                if (i == targetIndex)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(transform.position, path[i]);
                }
            }
        }

    }

}
