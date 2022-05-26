using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour, AI
{
    public static EnemyController instance;
    public float moveSpeed;
    private Vector3 nextPos;

    private void Awake()
    {
        instance = this;
    }

    public void MoveEnemy(List<Blocks> path, int i)
    {
        nextPos = new Vector3(path[i].transform.position.x, .5f, path[i].transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, nextPos, moveSpeed);
    }
}
