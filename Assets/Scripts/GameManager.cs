using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour {
	public Map map;
	public Farmer farmer;
	public int iguanaCount = 20;
	public Iguana iguanaPrefab;

	List<Iguana> _allIguanas = new List<Iguana>();

	public static GameManager instance { get; private set; }

	

	void Start () {
		Debug.Assert(FindObjectsOfType<GameManager>().Length == 1);
		instance = this;

		map.Build(farmer.transform.position);

		CreateIguanas();
	}

	void CreateIguanas() {
		
	}
}
