using UnityEngine;
using System.Collections;

public class Farmer : MonoBehaviour {
	public float linearSpeed = 5f;
	public float rotationSpeed = 90f;
	CharacterController _controller;
	Animator _animator;
	Vector3 _spawnPoint;

	protected void Start () {
		_controller = GetComponent<CharacterController>();
		_animator = GetComponent<Animator>();
		
	}
	
	void Update() {
		var motion = Input.GetAxis("Vertical") * linearSpeed;
		transform.Rotate(Vector3.up, Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime);
		_controller.SimpleMove(transform.forward * motion);
		_animator.SetFloat("Walk", motion);
	}

	public void Kill() {
		//ALUM: Respawn
		
	}

	void OnTriggerEnter(Collider collider) {
		if(collider.transform == transform)
			return;

		//ALUM: Manejar interacciones

		
	}
}
