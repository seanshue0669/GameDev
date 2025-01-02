using UnityEngine;

public class RouletteQuit : MonoBehaviour
{
    private static RouletteQuit _instance;
    public static RouletteQuit Instance => _instance ??= new RouletteQuit();

    public Scene.SceneLoader sceneLoader;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        EventSystem.Instance.RegisterEvent<int>("Roulette", "quit", Quit);
    }

    private void Quit(int tmp)
    {
        sceneLoader.LoadScene("Casion");
    }
}
