using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MuteBGMInStartMenu : MonoBehaviour
{
    private static HashSet<string> persistedObjects = new HashSet<string>();

    private bool tri = true;
    void Update()
    {


        if (SceneManager.GetActiveScene().name == "Start" || SceneManager.GetActiveScene().name == "Ending")
        {
            GetComponent<AudioSource>().Stop();
            tri = true;
        }
        else if (SceneManager.GetActiveScene().name == "Casion" && tri)
        {
            GetComponent<AudioSource>().Play(0);
            tri = false;
        }
            


    }
}
