using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;

public class PokerGameEvent : MonoBehaviour
{
    private static PokerGameEvent _instance;
    public static PokerGameEvent Instance => _instance ??= new PokerGameEvent();

    private bool isWaiting = false;

    private Vector3 cardPosPlayer;
    private Vector3 cardPosHost;

    private bool isPlayer;

    private GameObject[] Objects;

    public void Init(GameObject[] p_objects)
    {
        Debug.Log("Init PokerGameEvent");
        Objects = p_objects;
        Debug.Log(Objects.Length);

        EventSystem.Instance.RegisterEvent<string>("BJgame", "CardSpawn", CreateCard);

        EventSystem.Instance.RegisterEvent<bool>("BJgame", "CheckPlayer", SwapSlot);

        EventSystem.Instance.RegisterEvent<int>("BJgame", "DestroyCards", destroyCards);

        cardPosPlayer = new Vector3(-0.3f, 1f, -9.5f);

        cardPosHost = new Vector3(0.18f, 1f, -9.14f);

        isPlayer = true;
    }

    private void CreateCard(string card)
    {
        GameObject cardModel;
        for (int i = 0; i < 62; i++)
        {
            if (card == Objects[i].name)
            {
                cardModel = Objects[i];
                if (card == "Card_FunnyMemory")
                {
                    cardPosPlayer.y = 0f;
                }
                if (isPlayer)
                {
                    var cards = Instantiate(cardModel, cardPosPlayer, new Quaternion(-35f, 180f, 0, 1));
                    cards.gameObject.tag = "spawnedCards";
                    cardPosPlayer.x += 0.03f;
                }
                else
                {
                    var cards = Instantiate(cardModel, cardPosHost, new Quaternion(-35f, 180f, 0, 1));
                    cards.gameObject.tag = "spawnedCards";
                    cardPosHost.x += 0.03f;
                }
                if (card == "Card_FunnyMemory")
                {
                    cardPosPlayer.y = 1f;
                }

                break;
            }
        }
    }
    private void destroyCards(int cards)
    {

        Destroy(GameObject.FindWithTag("spawnedCards"));
    }
    private void SwapSlot(bool p)
    {
        isPlayer = p;
    }
}