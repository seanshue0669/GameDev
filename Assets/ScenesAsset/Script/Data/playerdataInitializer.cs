using UnityEngine;

public class playerdataInitializer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DataManager.Instance.playerData.SetValue("money", 100);
        DataManager.Instance.playerData.SetValue("chips", 0);
        DataManager.Instance.playerData.SetValue("canMoving", false);
        DataManager.Instance.playerData.SetValue("house", true);
        DataManager.Instance.playerData.SetValue("kidney", true);
        DataManager.Instance.playerData.SetValue("dignity", true);
        DataManager.Instance.playerData.SetValue("isRouletteSpinning", false);
    }
}
