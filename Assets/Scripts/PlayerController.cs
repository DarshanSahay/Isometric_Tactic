using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    public bool isMoving = false;

    [SerializeField] private float moveSpeed;
    [SerializeField] private MapGenerator map;
    [SerializeField] private Vector3 nextPos;


    private void Awake()
    {
        instance = this;
    }
    private void Update()
    {
        if (!isMoving)
        {
            GetCoordinatesOfMousePos();
        }
    }

    private void GetCoordinatesOfMousePos()
    {
        RaycastHit raycastHit;
        Ray raycast = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(raycast, out raycastHit) && raycastHit.collider.GetComponent<Blocks>() != null)
        {
            var block = raycastHit.collider.GetComponent<Blocks>();
            if (block && !block.isBlocked)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    Debug.Log(block.xPos + "," + block.zPos);
                    map.movePointX = (int)block.xPos;
                    map.movePointZ = (int)block.zPos;
                    map.findDistance = true;
                }
            }
        }
    }

    public void MovePlayer(List<Blocks> path, int i)
    {
        nextPos = new Vector3(path[i].transform.position.x, 0.5f, path[i].transform.position.z);
        Move(nextPos);
    }

    private void Move(Vector3 position)
    {
        transform.position = Vector3.MoveTowards(transform.position, position, moveSpeed); ;
    }
}
