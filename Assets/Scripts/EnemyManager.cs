using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    float speed = 1.0f;

    Transform target;
    Rigidbody rb;

	void Start () 
    {
        target = GameObject.Find("Truck").transform;
        rb = GetComponent<Rigidbody>();
        GetComponent<Animator>().speed = Random.Range(0.8f, 1.2f);
	}
	
	void FixedUpdate () 
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target.position - transform.position), 0.1f);
        rb.velocity = transform.forward * speed;
	}
}
