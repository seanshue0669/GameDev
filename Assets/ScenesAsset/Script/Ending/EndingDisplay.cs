using UnityEngine;

public class EndingDisplay : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject EndingScreen;
    public GameObject Ending1;
    public GameObject Ending2;
    public GameObject Ending3;
    private GameObject Ending;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Ending = Ending1;
            EndingScreen.SetActive(true);
            Ending.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Ending = Ending2;
            EndingScreen.SetActive(true);
            Ending.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Ending = Ending3;
            EndingScreen.SetActive(true);
            Ending.SetActive(true);
        }

        if (Input.GetMouseButtonDown(0))
        {
            EndingScreen.SetActive(false);
            Ending.SetActive(false);
        }
    }
}
