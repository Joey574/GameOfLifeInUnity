using UnityEngine;
using UnityEditor.SceneManagement;

public class AdvanceToGame : MonoBehaviour
{
    void onClick()
    {
        EditorSceneManager.LoadScene("Game", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
