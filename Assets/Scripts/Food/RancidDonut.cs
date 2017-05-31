using UnityEngine;
using System.Collections;

public class RancidDonut : MonoBehaviour {
	public Fly flyPrefab;
	

	void Start() {
		
	}

	public bool Exists() {
		return gameObject.activeSelf;
	}

	public void Eat(GameObject byWho) {
		gameObject.SetActive(false);
		
	}
}
