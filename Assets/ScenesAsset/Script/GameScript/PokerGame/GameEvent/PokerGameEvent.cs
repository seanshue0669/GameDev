using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;

public class PokerGameEvent : MonoBehaviour
{
    private static PokerGameEvent _instance;
    public static PokerGameEvent Instance => _instance ??= new PokerGameEvent();


    private Vector3 cardPosPlayer;
    private Vector3 cardPosHost;
    private float playerOffset;
    private float hostOffset;
    private bool des = true;

    private bool isPlayer;

    private GameObject[] Objects;

    public void Init(List<GameObject> p_objects)
    {
        Debug.Log("Init PokerGameEvent");
        Objects = p_objects.ToArray();
        Debug.Log(des);
        Debug.Log(Objects.Length);

        if (des)
        {
            EventSystem.Instance.RegisterEvent<string>("BJgame", "CardSpawn", CreateCard);

            EventSystem.Instance.RegisterEvent<bool>("BJgame", "CheckPlayer", SwapSlot);

            EventSystem.Instance.RegisterEvent<int>("BJgame", "DestroyCards", destroyCards);
            des = false;
        }
        

        cardPosPlayer = new Vector3(-0.34f, 0.95f, -9.15f);

        cardPosHost = new Vector3(-0.34f, 0.95f, -8.72f);
        playerOffset = 0.13f;
        hostOffset = 0.13f;

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
                    cardPosPlayer.x += 0.4f;
                    cardPosHost.y = 0f;
                    cardPosHost.x += 0.4f;
                }
                if (isPlayer)
                {
                    var cards = Instantiate(cardModel, cardPosPlayer + new Vector3(playerOffset, 0, 0), new Quaternion(-35f, 180f, 0, 1));
                    cards.gameObject.tag = "spawnedCards";
                    playerOffset += 0.13f;
                }
                else
                {
                    var cards = Instantiate(cardModel, cardPosHost + new Vector3(hostOffset, 0, 0), new Quaternion(-35f, 180f, 0, 1));
                    cards.gameObject.tag = "spawnedCards";
                    hostOffset += 0.13f;
                }
                if (card == "Card_FunnyMemory")
                {
                    cardPosPlayer.y = 1f;
                    cardPosPlayer.x -= 0.4f;
                    cardPosHost.y = 1f;
                    cardPosHost.x -= 0.4f;
                }

                break;
            }
        }

        /*for (int i=0; i<des; i++)
        {
            if (isPlayer)
            {
                playerOffset -= 0.13f;
                destroyRepetedCards(card);
            }
            else
            {
                hostOffset -= 0.13f;
                destroyRepetedCards(card);
            }
        }*/
    }

    private void destroyRepetedCards(string card)
    {
        Destroy(GameObject.Find(card + "(clone)"));
    }

    private void destroyCards(int cards)
    {
        playerOffset = 0.13f;
        hostOffset = 0.13f;
        Destroy(GameObject.FindWithTag("spawnedCards"));
    }
    private void SwapSlot(bool p)
    {
        isPlayer = p;
    }
}