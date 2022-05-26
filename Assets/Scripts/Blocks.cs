using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blocks : MonoBehaviour
{
    public float xPos, zPos;

    public bool isOcuppied = false;
    public bool isBlocked = false;
    public int visited;

    [SerializeField] private SpriteRenderer tileRenderer;
    public SpriteRenderer pathNotifier;
    public SpriteRenderer obstacleNotifier;

    [SerializeField] private GameObject obstacle;

    public void ObstacleCall()
    {
        ObstacleManager.instance.GenerateObstacle((int)xPos, (int)zPos);
    }

    private void OnMouseEnter()
    {
        if(!isBlocked)
        {
            tileRenderer.gameObject.SetActive(true);
        }
        else
        {
            obstacleNotifier.gameObject.SetActive(true);
        }
    }

    private void OnMouseExit()
    {
        tileRenderer.gameObject.SetActive(false);
        obstacleNotifier.gameObject.SetActive(false);
    }

    public void EnableObstacle(GameObject obs)
    {
        if (!isOcuppied)
        {
            if (!isBlocked)
            {
                obstacle = GameObject.Instantiate(obs, this.transform);
                var desiredPos = this.transform.position;
                desiredPos.y = 1;
                obstacle.transform.position = desiredPos;
                isBlocked = true;
            }
            else if (isBlocked)
            {
                ClearObstacle();
                isBlocked = false;
            }
            else
            {
                Debug.Log("ObstaclePresent");
            }
        }
        else
        {
            Debug.Log("Someone is there cant create obstacle");
        }
    }

    public void ClearObstacle()
    {
        isBlocked = false;
        DestroyImmediate(obstacle);
        return;
    }
}
