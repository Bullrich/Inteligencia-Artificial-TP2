using UnityEngine;
using System.Collections;

public class Farmer : MonoBehaviour
{
    public float linearSpeed = 5f;
    public float rotationSpeed = 90f;
    CharacterController _controller;
    Animator _animator;
    Vector3 _spawnPoint;

    public bool _playing;

    protected void Start()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _spawnPoint = transform.position;
    }

    void Update()
    {
        if (_playing)
        {
            var motion = Input.GetAxis("Vertical") * linearSpeed;
            transform.Rotate(Vector3.up, Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime);
            _controller.SimpleMove(transform.forward * motion);
            _animator.SetFloat("Walk", motion);
            if (Input.GetKeyDown(KeyCode.K))
                GameManager.instance.MadeSound(transform.position);
        }
    }

    public void Kill()
    {
        //ALUM: Respawn
        transform.position = _spawnPoint;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Iguanas"))
            Kill();
        else if (collision.gameObject.GetComponent<NoiseSource>() != null){
            GameManager.instance.MadeSound(transform.position);
            collision.gameObject.GetComponent<NoiseSource>().Play();
        }

        //ALUM: Manejar interacciones


    }
}
