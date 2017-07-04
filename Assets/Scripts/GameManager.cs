using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Blue.Pathfinding;

public class GameManager : MonoBehaviour
{
    public Farmer farmer;
    public int iguanaCount = 20;
    public Iguana iguanaPrefab;

    List<Iguana> _allIguanas = new List<Iguana>();
    public Grid grid;

    public static GameManager instance { get; private set; }

[Tooltip("Distance that lizards hears the sound")]
    public int soundDistance = 30;



    void Start()
    {
        Debug.Assert(FindObjectsOfType<GameManager>().Length == 1);
        instance = this;

        StartCoroutine(CreateIguanas());
    }

    IEnumerator CreateIguanas()
    {
        // Need a pause to allow the grid to be build
        // Here would go a loading screen
        yield return new WaitUntil(()=> grid.gridCreated());
        for (int i = 0; i < iguanaCount; i++)
        {
            GameObject iguana = Instantiate(iguanaPrefab.gameObject, grid.FindRandomWalkableNode().worldPosition, transform.rotation);
            _allIguanas.Add(iguana.GetComponent<Iguana>());
        }
        yield return new WaitForSeconds(.5f);
        farmer._playing = true;
    }

    public void MadeSound(Vector3 soundPos)
    {
        foreach (Iguana ig in _allIguanas)
            if (PathRequestManager.GetDistanceFromPoints(ig.transform.position, soundPos) < soundDistance)
                ig.HeardSound(farmer.transform.position);
    }
}
