using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MinionAI : MonoBehaviour
{

    public Transform Target;
    int MoveSpeed = 4;
    int MaxDist = 10;
    int MinDist = 3;
    public Vector3 myvector;


    void Start()
    {
        
    }

    void Update()
    {
        transform.LookAt(Target);

        if (Vector3.Distance(transform.position, Target.position) >= MinDist)
        {

            transform.position += transform.forward * MoveSpeed * Time.deltaTime;




            if (Vector3.Distance(transform.position, Target.position) <= MaxDist)
            {
                //Here Call any function U want Like Shoot at here or something
            }

        }
    }
}