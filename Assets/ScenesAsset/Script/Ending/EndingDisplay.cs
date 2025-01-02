using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingDisplay : MonoBehaviour
{
    public GameObject EndingScreen;
    public GameObject Ending1;
    public GameObject Ending2;
    public GameObject Ending3;
    private GameObject Ending;

    private int richEnding = 2000;
    private int poorEnding = 0;


    private void Start()
    {
        if (DataManager.Instance.playerData.GetValue<int>("money") > richEnding)
        {
            Ending = Ending1;
            Ending.SetActive(true);
            AchievementManager.instance.Unlock("Big money");
        }
        else if (DataManager.Instance.playerData.GetValue<int>("money") < poorEnding)
        {
            Ending = Ending2;
            Ending.SetActive(true);
            AchievementManager.instance.Unlock("Be poor");
        }
        else
        {
            Ending = Ending3;
            Ending.SetActive(true);
            AchievementManager.instance.Unlock("Financially responsible");
        }
    }
    void Update()
    {
        

        if (Input.GetMouseButtonDown(0))
        {
            Ending.SetActive(false);
            SceneManager.LoadSceneAsync("casion");
        }
    }
}
