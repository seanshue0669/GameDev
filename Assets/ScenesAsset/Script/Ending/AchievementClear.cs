using UnityEngine;

public class AchievementClear : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AchievementManager.instance.ResetAchievementState();
    }

    
}
