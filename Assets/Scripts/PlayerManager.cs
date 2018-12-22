using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour 
{
    Transform liftPos;
    Transform throwPos;
    bool isLift = false;
    bool isActive = false;

    EnemyManager enemy;
    MarkerManager marker;

	void Start ()
    {
        liftPos = transform.FindChild("LiftPos").transform;
        throwPos = transform.FindChild("ThrowPos").transform;

        marker = transform.GetComponentInChildren<MarkerManager>();
	}

    public bool IsActive
    {
        get
        {
            return isActive;
        }
        set
        {
            isActive = value;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(isActive && !isLift && collision.transform.tag.Equals("Enemy"))
        {
            enemy = collision.transform.GetComponent<EnemyManager>();

            if(enemy)
            {
                isLift = true;

                marker.SetMaterial();

                enemy.Lift();
                enemy.transform.parent = liftPos;
                enemy.transform.position = liftPos.position;
                enemy.transform.eulerAngles = new Vector3(-90f, transform.eulerAngles.y, transform.eulerAngles.z);
            }
        }
    }

    void Update () 
    {
        if(isActive && isLift)
        {
            if(enemy)
            {
                if (Input.GetButtonDown("Throw"))
                {
                    marker.ResetMaterial();

                    enemy.transform.parent = null;
                    enemy.Throw(throwPos);

                    enemy = null;

                    StartCoroutine(WaitForThrow());
                }
            }
        }
	}

    IEnumerator WaitForThrow()
    {
        yield return new WaitForSeconds(0.1f);

        isLift = false;
    }
}
