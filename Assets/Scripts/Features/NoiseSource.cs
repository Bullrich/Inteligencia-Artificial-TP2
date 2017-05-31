using UnityEngine;
using System.Collections;

public class NoiseSource : MonoBehaviour {

	public AudioClip[] noiseClip;
	AudioSource _asrc;
	float _cooldown = 0f;

	public void Play() {
		if(_cooldown > Time.time)
			return;

		_cooldown = Time.time + 2f;
		Debug.Log("Noise play for " + name);

		_asrc.clip = noiseClip[Random.Range(0, noiseClip.Length)];
		_asrc.Play();

		//ALUM: Avisar a iguanas
		
	}

	void Start () {
		Debug.Assert(noiseClip.Length > 0, "No noise clip for " + name);
		_asrc = gameObject.AddComponent<AudioSource>();
		_asrc.minDistance = 10f;
	}

}
