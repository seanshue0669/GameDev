using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QuitMinigameToCasino : MonoBehaviour
{
    [SerializeField] private Button QuitButton;

    void Start()
    {
        QuitButton.onClick.AddListener(TaskOnClick);
    }
    void TaskOnClick()
    {
        SceneManager.LoadSceneAsync("Casion");
    }
}
