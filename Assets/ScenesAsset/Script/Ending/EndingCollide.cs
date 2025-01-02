using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndingCollide : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        SceneManager.LoadSceneAsync("Ending");
    }

}
