using UnityEngine;
using UnityEditor;

public class ObstacleEditor : EditorWindow
{
    static int columns = 10;
    static int rows = 10;

    [MenuItem("Tools/Obstacle Spawner")]
    public static void ShowWindow()
    {
        GetWindow(typeof(ObstacleEditor));
    }

    private void OnGUI()
    {
        GUILayout.Label("Generate Obstacle", EditorStyles.boldLabel);

        for (int i = columns - 1; i >= 0; i--)
        {
            GUILayout.BeginHorizontal();
            for (int j = 0; j < rows; j++)
            {
                if (GUILayout.Button(j.ToString() + "," + i.ToString()))
                {
                    ObstacleManager.instance.GenerateObstacle(j, i);
                }
            }
            GUILayout.EndHorizontal();
        }
    }
}
