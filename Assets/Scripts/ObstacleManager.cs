using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObstacleManager : MonoBehaviour
{
    [SerializeField] private MapGenerator map;
    [SerializeField] private ObstacleSO obstacleSO;
    [SerializeField] private GameObject obstacleMenuPanel;
    [SerializeField] private Button[] interactingButtons;

    public static ObstacleManager instance;
    public void Awake()
    {
        instance = this;
    }

    public void OpenObstacleManager()
    {
        obstacleMenuPanel.gameObject.SetActive(true);
        interactingButtons[0].gameObject.SetActive(true);
        interactingButtons[1].gameObject.SetActive(false);
    }
    public void CloseObstacleManager()
    {
        obstacleMenuPanel.gameObject.SetActive(false);
        interactingButtons[0].gameObject.SetActive(false);
        interactingButtons[1].gameObject.SetActive(true);
    }

    public void StopButtonInteraction()
    {
        foreach (Button item in interactingButtons)
        {
            item.interactable = false;
        }
    }

    public void ResetButtonInteraction()
    {
        foreach (Button item in interactingButtons)
        {
            item.interactable = true;
        }
    }
    public void GenerateObstacle(int posX, int posZ)
    {
        if (map.blocks[posX, posZ] != null)
        {
            map.blocks[posX, posZ].GetComponent<Blocks>().EnableObstacle(obstacleSO.obstacleType);
        }
    }
}
