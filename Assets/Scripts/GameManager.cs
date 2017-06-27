using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public Farmer farmer;
    public int iguanaCount = 20;
    public Iguana iguanaPrefab;

    List<Iguana> _allIguanas = new List<Iguana>();

    public static GameManager instance { get; private set; }



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
        yield return new WaitForSeconds(1.5f);
        for (int i = 0; i < iguanaCount; i++)
        {
            GameObject iguana = Instantiate(iguanaPrefab.gameObject, new Vector3(Random.Range(-20, 20), transform.position.y, Random.Range(-20, 20)), transform.rotation);
            _allIguanas.Add(iguana.GetComponent<Iguana>());
        }
		yield return new WaitForSeconds(.5f);
		farmer._playing=true;
    }

    public void FoundPlayer()
    {
        foreach (Iguana ig in _allIguanas)
            ig.FoundPlayer(farmer.transform.position);
    }
}
