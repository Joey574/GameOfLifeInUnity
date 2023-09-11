using UnityEngine;
using UnityEditor.SceneManagement;

public class AdvanceToGame : MonoBehaviour
{
    void Awake()
    {
        EditorSceneManager.LoadScene("Game", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
