using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MoveDirection
{
    Up,
    Right,
    Down,
    Left
}

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private Button moveButton;

    [SerializeField] private int size;
    [SerializeField] private int scale;

    [SerializeField] private Button[,] buttons = new Button[10, 10];
    [SerializeField] private Button button;
    [SerializeField] private GameObject buttonParent;

    [SerializeField] private ObstacleManager obstacleManager;

    [SerializeField] private Blocks cube;
    [SerializeField] private Camera cam;
    [SerializeField] private Text posText;
    [SerializeField] private Text occText;
    [SerializeField] private Text bloText;

    [SerializeField] private int playerX = 0;
    [SerializeField] private int playerZ = 0;

    [SerializeField] private int enemyX = 0;
    [SerializeField] private int enemyZ = 0;

    private bool findEnemyDistance = false;
    private List<Blocks> playerPath = new List<Blocks>();
    private List<Blocks> enemyPath = new List<Blocks>();
    private List<Blocks> tempList = new List<Blocks>();

    public Blocks[,] blocks;
    public bool findDistance = false;

    public int movePointX;
    public int movePointZ;

    void Start()
    {
        moveButton.onClick.AddListener(Movement);
        blocks = new Blocks[size, size];

        CreateButtons();
        GenerateBlocks();

        PlayerInitialization();
        EnemyInitialization();
    }

    private void Update()
    {
        GetBlockInfo();

        if (findDistance)
        {
            InitialSetup();
            SetDistance();
            SetPath(playerPath);
            findDistance = false;
        }

        if (findEnemyDistance)
        {
            InitialSetupEnemy();
            SetDistance();
            SetPath(enemyPath);
            findEnemyDistance = false;
        }
    }

    void CreateButtons()
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                buttons[i, j] = Instantiate(button, buttonParent.transform);
                buttons[i, j].GetComponentInChildren<Text>().text = i + " - " + j;
            }
        }
    }

    void GenerateBlocks()
    {
        for (float i = 0; i < size; i++)
        {
            for (float j = 0; j < size; j++)
            {
                blocks[(int)i, (int)j] = Instantiate<Blocks>(cube, new Vector3(i, 0, j), Quaternion.identity);
                Blocks b = blocks[(int)i, (int)j].GetComponent<Blocks>();
                b.xPos = i;
                b.zPos = j;
                b.Test(buttons[(int)i, (int)j]);
            }
        }
    }

    void GetBlockInfo()
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            Transform objectHit = hit.transform;
            Blocks getBlock = objectHit.GetComponent<Blocks>();
            posText.text = "Tile Position : " + getBlock.xPos + " - " + getBlock.zPos;
            occText.text = "Ocuppied : " + getBlock.isOcuppied.ToString();
            bloText.text = "Blocked : " + getBlock.isBlocked.ToString();
        }
    }

    private void InitialSetup()
    {
        foreach (Blocks obj in blocks)
        {
            obj.visited = -1;
        }
        blocks[playerX, playerZ].visited = 0;
        blocks[enemyX, enemyZ].visited = -1;
    }

    private void InitialSetupEnemy()
    {
        foreach (Blocks obj in blocks)
        {
            obj.visited = -1;
        }
        blocks[enemyX, enemyZ].visited = 0;
        blocks[enemyX, enemyZ].isBlocked = true;
    }

    private void PlayerInitialization()
    {
        blocks[playerX, playerZ].isOcuppied = true;
    }

    private void EnemyInitialization()
    {
        blocks[enemyX, enemyZ].isBlocked = true;
    }

    public void Movement()
    {
        StartCoroutine(MovePlayer());
    }

    public IEnumerator MovePlayer()
    {
        int i = playerPath.Count - 2;

        PlayerController.instance.isMoving = true;
        moveButton.interactable = false;
        obstacleManager.StopButtonInteraction();

        while (i >= 0)
        {
            PlayerController.instance.MovePlayer(playerPath, i, true);
            i--;
            yield return new WaitForSeconds(.5f);
        }
        yield return null;
        blocks[playerX, playerZ].isOcuppied = false;

        playerX = (int)playerPath[0].xPos;
        playerZ = (int)playerPath[0].zPos;

        blocks[playerX, playerZ].isOcuppied = true;

        foreach (Blocks item in playerPath)
        {
            item.pathNotifier.gameObject.SetActive(false);
        }

        moveButton.interactable = true;
        obstacleManager.ResetButtonInteraction();
        findEnemyDistance = true;

        yield return null;

        StartCoroutine(MoveEnemy());
    }

    public IEnumerator MoveEnemy()
    {
        int i = enemyPath.Count - 1;

        while (i >= 1)
        {
            EnemyController.instance.MoveEnemy(enemyPath, i);
            i--;
            yield return new WaitForSeconds(.5f);
        }

        yield return null;
        blocks[enemyX, enemyZ].isBlocked = false;
        enemyX = (int)enemyPath[1].xPos;
        enemyZ = (int)enemyPath[1].zPos;

        blocks[enemyX, enemyZ].isBlocked = true;
        PlayerController.instance.isMoving = false;
        yield return null;
    }

    void SetDistance()
    {
        for (int step = 1; step < size * size; step++)
        {
            foreach (Blocks newBlock in blocks)
            {
                if (newBlock.visited == step - 1)
                {
                    TestFourDirections((int)newBlock.xPos, (int)newBlock.zPos, step);
                }
            }
        }
    }

    void SetPath(List<Blocks> path)
    {
        foreach (Blocks item in path)
        {
            item.pathNotifier.gameObject.SetActive(false);
        }

        int step;
        int x = movePointX;
        int y = movePointZ;

        path.Clear();

        if (blocks[x, y] && blocks[x, y].visited > 0)
        {
            path.Add(blocks[x, y]);
            step = blocks[x, y].visited - 1;
        }
        else
        {
            Debug.Log("Cant Reach");
            return;
        }
        for (int i = step; i > -1; i--)
        {
            if (TestDirection(x, y, i, MoveDirection.Up))
            {
                tempList.Add(blocks[x, y + 1]);
            }
            if (TestDirection(x, y, i, MoveDirection.Right))
            {
                tempList.Add(blocks[x + 1, y]);
            }
            if (TestDirection(x, y, i, MoveDirection.Down))
            {
                tempList.Add(blocks[x, y - 1]);
            }
            if (TestDirection(x, y, i, MoveDirection.Left))
            {
                tempList.Add(blocks[x - 1, y]);
            }
            Blocks tempObj = FindClosest(blocks[movePointX, movePointZ].transform, tempList);

            path.Add(tempObj);
            x = (int)tempObj.xPos;
            y = (int)tempObj.zPos;
            tempList.Clear();
        }

        foreach (Blocks block in path)
        {
            block.pathNotifier.gameObject.SetActive(true);
        }
    }

    bool TestDirection(int x, int y, int step, MoveDirection direction)
    {
        switch (direction)
        {
            case MoveDirection.Up:
                if (y + 1 < size && blocks[x, y + 1] && blocks[x, y + 1].visited == step && !blocks[x, y + 1].isBlocked)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case MoveDirection.Right:
                if (x + 1 < size && blocks[x + 1, y] && blocks[x + 1, y].visited == step && !blocks[x + 1, y].isBlocked)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case MoveDirection.Down:
                if (y - 1 > -1 && blocks[x, y - 1] && blocks[x, y - 1].visited == step && !blocks[x, y - 1].isBlocked)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case MoveDirection.Left:
                if (x - 1 > -1 && blocks[x - 1, y] && blocks[x - 1, y].visited == step && !blocks[x - 1, y].isBlocked)
                {
                    return true;
                }
                else
                {
                    return false;
                }
        }

        return false;
    }

    private void TestFourDirections(int x, int y, int step)
    {
        if (TestDirection(x, y, -1, MoveDirection.Up))
        {
            SetVisited(x, y + 1, step);
        }
        if (TestDirection(x, y, -1, MoveDirection.Right))
        {
            SetVisited(x + 1, y, step);
        }
        if (TestDirection(x, y, -1, MoveDirection.Down))
        {
            SetVisited(x, y - 1, step);
        }
        if (TestDirection(x, y, -1, MoveDirection.Left))
        {
            SetVisited(x - 1, y, step);
        }

    }
    void SetVisited(int x, int y, int step)
    {
        if (blocks[x, y])
        {
            blocks[x, y].visited = step;
        }
    }

    Blocks FindClosest(Transform targetLoc, List<Blocks> list)
    {
        float currentDistance = scale * size * size;
        int indexNumber = 0;
        for (int i = 0; i < list.Count; i++)
        {
            if (Vector3.Distance(targetLoc.position, list[i].transform.position) < currentDistance)
            {
                currentDistance = Vector3.Distance(targetLoc.position, list[i].transform.position);
                indexNumber = i;
            }
        }
        return list[indexNumber];
    }
}
