using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    [SerializeField] private MapGenerator map;
    [SerializeField] private ObstacleSO obstacleSO;

    public static ObstacleManager instance;
    public void Awake()
    {
        instance = this;
    }
    public void GenerateObstacle(int posX, int posZ)
    {
        if (map.blocks[posX, posZ] != null)
        {
            map.blocks[posX, posZ].GetComponent<Blocks>().EnableObstacle(obstacleSO.obstacleType);
        }
    }
}
