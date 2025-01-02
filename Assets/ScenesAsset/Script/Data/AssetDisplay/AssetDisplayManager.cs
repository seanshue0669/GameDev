using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class AssetDisplayManager : MonoBehaviour
{
    private static AssetDisplayManager _instance;
    public static AssetDisplayManager Instance => _instance ??= new AssetDisplayManager();

    private TMP_Text Asset;

    private void Awake()
    {
        EventSystem.Instance.RegisterEvent<int>("Assetdisplay", "display", Show);
    }
    
    void Show(int tmp)
    {
        GameObject FindObject = GameObject.FindWithTag("AssetUI");
        if (FindObject == null)
        {
            Debug.LogError("cant find RouletteObject!!!????");
        }

        Asset = TransformExtensions.FindChildComponent<TMP_Text>(FindObject.transform, "AssetDisplay");

        int money = DataManager.Instance.playerData.GetValue<int>("money");
        int chips = DataManager.Instance.playerData.GetValue<int>("chips");

        Asset.text = $"Money : {money}\nChips : {chips}";
    }

}
