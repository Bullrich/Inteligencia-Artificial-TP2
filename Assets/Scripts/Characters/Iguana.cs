using UnityEngine;
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

    Vector3 _startPatrol, _endPatrol;
    bool _hasRequestedPath = false;



    protected void Start()
    {
        Debug.Assert(helmetPrefabs.Length == rankProbability.Length);
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _los = GetComponent<LineOfSight>();
        Invoke("StartNow", 1f);
    }

    private void StartNow()
    {
		Vector3 targetPosition = transform.position + new Vector3(2, 0,0);
		print(targetPosition);
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
			foreach(Vector3 vec in newPath)
			print(vec);
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
                break;
        }


        if (delta.sqrMagnitude > 0.1f)
        {
            var motionMagnitude = _maxSpeed;

            //ALUM: Truncar la velocidad a una velocidad máxima y animar con la velocidad real (en vez de _maxSpeed)


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
        if (Vector3.Distance(path[targetIndex], transform.position) > .3f)
        {
            var offset = path[targetIndex] - transform.position;
            if (offset.magnitude > .1f)
            {
                offset = offset.normalized * _maxSpeed;
                _controller.Move(offset * Time.deltaTime);
            }
        }
        else if (path.Length < targetIndex)
            targetIndex++;
        else if (!_hasRequestedPath)
        {
            // Restart the route
            if (Vector3.Distance(_startPatrol, transform.position) < 1)
                GetPatrolRoute(_endPatrol);
            else
                GetPatrolRoute(_startPatrol);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = _gizmoColor;                     //Color único por iguana
        var offset = Vector3.one * 0.15f * _iguanaIndex;    //Un pequeño offset para que no este todo superpuesto (usando el índice de la iguana)

        Gizmos.DrawSphere(transform.position + offset, 0.5f);   //Esfera sobre la iguana para saber el color asociado a la misma

        //ALUM: Dibujar la ruta de patrullaje o la que se está siguiendo de momento. Utilizar "offset" para no superponer los gizmos.


    }

}
