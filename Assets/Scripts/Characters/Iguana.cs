using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Blue.Pathfinding;
using Blue.Fov;
using Blue.Tree;

[RequireComponent(typeof(FieldOfView))]
public class Iguana : MonoBehaviour
{


    public enum Input
    {
        TargetSeen,
        TargetNotSeen,
        FollowNoise,
        ReachedDestination
    }
    public enum State
    {
        Patrolling, Following, Seeking, Returning
    }

    public State currentState;
    public Input currentInput;

    [Range(3, 50)]
    public int waypointsInPatrol = 15;

    public GameObject[] helmetPrefabs;
    public Transform helmetPos;
    public float[] rankProbability;
    public int rank;
    public float _maxSpeed;

    int _iguanaIndex;
    public Color _gizmoColor;
    CharacterController _controller;
    Animator _animator;


    Vector3[] path, tempPath;
    private int pathIndex, tempIndex;
    private Vector3 currentTarget;

    bool patrollingInversed, hasAskedForPath;

    private FieldOfView _fov;

    private Vector3 soundSrc;


    protected void Start()
    {
        Debug.Assert(helmetPrefabs.Length == rankProbability.Length);
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _fov = GetComponent<FieldOfView>();
        currentTarget = transform.position;
        _fov.ContinueFOV();
        _gizmoColor = Random.ColorHSV();
        GetRandomRoute();
    }

    void ProcessInput(Input input)
    {
        if (currentInput == input)
            return;
        else
            currentInput = input;

        switch (input)
        {
            case Input.TargetNotSeen:
                tempPath = null;
                StartCoroutine(GetRoute(path[pathIndex]));
                currentState = State.Returning;
                break;
            case Input.TargetSeen:
                Scream();
                currentState = State.Seeking;
                break;
            case Input.FollowNoise:
                tempPath = null;
                StartCoroutine(GetRoute(currentTarget));
                currentState = State.Following;
                break;
            case Input.ReachedDestination:
                if (currentState == State.Returning)
                    currentState = State.Patrolling;
                else if (currentState == State.Following)
                {
                    currentState = State.Returning;
                    tempPath = null;
                    StartCoroutine(GetRoute(path[pathIndex]));
                }
                break;
        }
    }

    private void Behavior()
    {
        switch (currentState)
        {
            case State.Following:
                GoToDestination();
                break;
            case State.Patrolling:
                Patrolling();
                break;
            case State.Returning:
                GoToDestination();
                break;
            case State.Seeking:
                Transform target = _fov.getTarget();
                if (target != null)
                    MoveToPoint(target.position);
                else
                    ProcessInput(Input.TargetNotSeen);
                break;
        }
    }

    private void GoToDestination()
    {
        if (tempPath == null)
            return;
        //failsafe
        else if (tempPath.Length == 0)
            ProcessInput(Input.ReachedDestination);

        try
        {
            if (hasReachedDestination(tempPath[tempPath.Length - 1]))
            {
                ProcessInput(Input.ReachedDestination);
                tempPath = null;
            }
            FollowPath(tempPath, ref tempIndex);
        }
        catch (System.Exception e)
        {
            Debug.Log(string.Format("State: {0}, input: {1}, path:{2}/{3}", currentState, currentInput, tempIndex, tempPath.Length));
            Debug.LogError(e);
        }
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

    public void HeardSound(Vector3 _soundSrc)
    {
        currentTarget = _soundSrc;
        ProcessInput(Input.FollowNoise);
    }

    void Scream()
    {
        _animator.SetTrigger("Hit");
        GameManager.instance.MadeSound(_fov.getTarget().position);
        //ALUM: Generar "ruido"

    }

    private void GetRandomRoute()
    {
        PathRequestManager.RequestRandomPath(transform.position, waypointsInPatrol, OnPathFound);
    }

    private IEnumerator GetRoute(Vector3 targetPosition)
    {
        // Wait until we have our last path assigned so that we don't collapse the queue
        yield return new WaitUntil(() => !hasAskedForPath);
        PathRequestManager.RequestPath(transform.position, targetPosition, OnPathFound);
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            if (path == null)
            {
                path = newPath;
                pathIndex = 0;
                ProcessInput(Input.TargetNotSeen);
            }
            else
            {
                tempPath = newPath;
                tempIndex = 0;
            }
        }
        else
            GetRandomRoute();
        hasAskedForPath = false;
    }



    // Update is called once per frame
    protected void Update()
    {
        //ALUM: Alimentar la FSM con sensores
        if (_fov.hasTargetInView())
        {
            ProcessInput(Input.TargetSeen);
        }

        //ALUM: Moverse acorde al estado.
        Vector3 delta = Vector3.zero;

        Behavior();


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

    void FollowPath(Vector3[] currentPath, ref int currentIndex, bool increaseIndex = true)
    {
        if (currentPath == null)
            return;
        try
        {
            if (Vector3.Distance(currentPath[currentIndex], transform.position) > 1)
            {
                var offset = currentPath[currentIndex] - transform.position;
                if (offset.magnitude > .1f)
                {
                    currentTarget = currentPath[currentIndex];
                    MoveToPoint(currentTarget);
                    transform.LookAt(currentTarget);
                }
            }
            else
                currentIndex += (increaseIndex ? 1 : -1);
        }
        catch (System.Exception e)
        {
            Debug.LogError(string.Format("State: {0}, index: {1}/{2}", currentState, currentIndex, currentPath.Length));
            Debug.LogError(e);
        }
    }

    public bool hasReachedDestination(Vector3 destination)
    {
        return Vector3.Distance(destination, transform.position) <= 1.3f;
    }

    private void Patrolling()
    {
        if (path == null)
            return;
        if (pathIndex >= path.Length || pathIndex < 0)
        {
            patrollingInversed = !patrollingInversed;
            pathIndex += (patrollingInversed ? -1 : 1);
        }

        FollowPath(path, ref pathIndex, !patrollingInversed);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = _gizmoColor;                     //Color único por iguana
        var offset = Vector3.one * 0.15f * _iguanaIndex;    //Un pequeño offset para que no este todo superpuesto (usando el índice de la iguana)

        Gizmos.DrawSphere(transform.position + offset, 0.5f);   //Esfera sobre la iguana para saber el color asociado a la misma

        //ALUM: Dibujar la ruta de patrullaje o la que se está siguiendo de momento. Utilizar "offset" para no superponer los gizmos.
        if (currentState == State.Following || currentState == State.Patrolling || currentState == State.Returning)
        {
            Vector3[] _path = (currentState == State.Patrolling ? path : tempPath);
            int _index = (currentState == State.Patrolling ? pathIndex : tempIndex);
            if (_path != null)
            {
                for (int i = 0; i < _path.Length; i++)
                {
                    Gizmos.DrawCube(_path[i], Vector3.one);

                    if (i == _index)
                    {
                        Gizmos.DrawLine(transform.position, _path[i]);
                    }
                }
            }
        }
    }

}
