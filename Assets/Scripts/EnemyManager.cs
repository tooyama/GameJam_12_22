using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    int id = 0;
    [SerializeField]
    float speed = 1.0f;
    [SerializeField]
    GameObject bombEffect;

    Transform target;
    Rigidbody rb;

    GameManager gameManager;

    float defaultSpeed;
    bool isLifted = false;
    bool isDead = false;

    void OnCollisionEnter(Collision collision)
    {
        if(isLifted && collision.transform.tag.Equals("Floor"))
        {
            isLifted = false;
            transform.eulerAngles = Vector3.zero;
        }

        if(collision.transform.tag.Equals("Enemy"))
        {
            EnemyManager enemy = collision.transform.GetComponent<EnemyManager>();

            if(isLifted && !enemy.IsLifted && id == enemy.ID)
            {
                enemy.Dead();
                Dead();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Field"))
        {
            speed = defaultSpeed;
        }
    }

    public int ID
    {
        get
        {
            return id;
        }
        private set 
        {
            id = value; 
        }
    }

    public bool IsLifted
    {
        get
        {
            return isLifted;
        }
        private set
        {
            isLifted = value;
        }
    }

    public void AddSpeed()
    {
        speed += 0.01f;
    }

    public void Dead()
    {
        isDead = true;
        gameManager.AddScore();
        Instantiate(bombEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    public void Lift()
    {
        isLifted = true;
        rb.isKinematic = true;
    }

    public void Throw(Transform throwPos)
    {
        rb.isKinematic = false;
        Shoot(throwPos.position);
    }

    void Shoot(Vector3 i_targetPosition)
    {
        ShootFixedSpeedInPlaneDirection(i_targetPosition, 0.5f);
    }

    void ShootFixedSpeedInPlaneDirection(Vector3 i_targetPosition, float i_speed)
    {
        if (i_speed <= 0.0f)
        {
            return;
        }

        Vector2 startPos = new Vector2(transform.position.x, transform.position.z);
        Vector2 targetPos = new Vector2(i_targetPosition.x, i_targetPosition.z);
        float distance = Vector2.Distance(targetPos, startPos);

        float time = distance / i_speed;

        ShootFixedTime(i_targetPosition, time);
    }

    void ShootFixedTime(Vector3 i_targetPosition, float i_time)
    {
        float speedVec = ComputeVectorFromTime(i_targetPosition, i_time);
        float angle = ComputeAngleFromTime(i_targetPosition, i_time);

        if (speedVec <= 0.0f)
        {
            return;
        }

        Vector3 vec = ConvertVectorToVector3(speedVec, angle, i_targetPosition);
        InstantiateShootObject(vec);
    }

    void InstantiateShootObject(Vector3 i_shootVector)
    {
        Vector3 force = i_shootVector * rb.mass;

        rb.AddForce(force, ForceMode.Impulse);
    }

    float ComputeVectorFromTime(Vector3 i_targetPosition, float i_time)
    {
        Vector2 vec = ComputeVectorXYFromTime(i_targetPosition, i_time);

        float v_x = vec.x;
        float v_y = vec.y;

        float v0Square = v_x * v_x + v_y * v_y;

        if (v0Square <= 0.0f)
        {
            return 0.0f;
        }

        float v0 = Mathf.Sqrt(v0Square);

        return v0;
    }

    float ComputeAngleFromTime(Vector3 i_targetPosition, float i_time)
    {
        Vector2 vec = ComputeVectorXYFromTime(i_targetPosition, i_time);

        float v_x = vec.x;
        float v_y = vec.y;

        float rad = Mathf.Atan2(v_y, v_x);
        float angle = rad * Mathf.Rad2Deg;

        return angle;
    }

    Vector2 ComputeVectorXYFromTime(Vector3 i_targetPosition, float i_time)
    {
        if (i_time <= 0.0f)
        {
            return Vector2.zero;
        }

        Vector2 startPos = new Vector2(transform.position.x, transform.position.z);
        Vector2 targetPos = new Vector2(i_targetPosition.x, i_targetPosition.z);
        float distance = Vector2.Distance(targetPos, startPos);

        float x = distance;

        float g = -Physics.gravity.y;
        float y0 = transform.position.y;
        float y = i_targetPosition.y;
        float t = i_time;

        float v_x = x / t;
        float v_y = (y - y0) / t + (g * t) / 2;

        return new Vector2(v_x, v_y);
    }

    Vector3 ConvertVectorToVector3(float i_v0, float i_angle, Vector3 i_targetPosition)
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = i_targetPosition;
        startPos.y = 0.0f;
        targetPos.y = 0.0f;

        Vector3 dir = (targetPos - startPos).normalized;
        Quaternion yawRot = Quaternion.FromToRotation(Vector3.right, dir);
        Vector3 vec = i_v0 * Vector3.right;

        vec = yawRot * Quaternion.AngleAxis(i_angle, Vector3.forward) * vec;

        return vec;
    }

    void Start () 
    {
        target = GameObject.Find("Truck").transform;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        rb = GetComponent<Rigidbody>();
        GetComponent<Animator>().speed = Random.Range(0.8f, 1.2f);

        defaultSpeed = speed;
        speed = defaultSpeed * 2.0f;
	}
	
	void FixedUpdate () 
    {
        if (!isLifted)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target.position - transform.position), 0.1f);
            rb.velocity = transform.forward * speed;
        }
	}
}
