using UnityEngine;
using TMPro;


public class Interaction : MonoBehaviour
{
    public float interactDistance = 3f; // 交互距离
    public LayerMask miniGameLayer;
    public LayerMask npcLayer;
    public TextMeshProUGUI interactText; // UI 文本，拖拽到 Inspector 中
    public Scene.SceneLoader sceneLoader;

    private GameObject currentBox = null; // 当前目标箱子

    void Update()
    {
        RaycastHit hit;
        // 从摄像机的位置向前发射射线
        if (Physics.Raycast(transform.position, transform.forward, out hit, interactDistance, miniGameLayer))
        {
            string tag = hit.collider.tag;

            if (tag == "Dicegame")
            {
                currentBox = hit.collider.gameObject;
                // 显示交互提示
                if (interactText != null)
                {
                    interactText.gameObject.SetActive(true);
                    interactText.text = "Press E to play DiceGame";
                }

                // 检测是否按下 E 键
                if (Input.GetKeyDown(KeyCode.E))
                {
                    UnityEngine.Cursor.lockState = CursorLockMode.None;
                    sceneLoader.LoadScene("DiceScene");
                    //Debug.Log("DiceScecne");
                }
                return; // 如果有命中，结束 Update
            }
            else if (tag == "Blackjack")
            {
                currentBox = hit.collider.gameObject;
                // 显示交互提示
                if (interactText != null)
                {
                    interactText.gameObject.SetActive(true);
                    interactText.text = "Press E to play Blackjack";
                }

                // 检测是否按下 E 键
                if (Input.GetKeyDown(KeyCode.E))
                {
                    UnityEngine.Cursor.lockState = CursorLockMode.None;
                    sceneLoader.LoadScene("PokerScene");
                    //Debug.Log("Black");
                }
                return; // 如果有命中，结束 Update
            }
            else if (tag == "RouletteGame")
            {
                currentBox = hit.collider.gameObject;
                // 显示交互提示
                if (interactText != null)
                {
                    interactText.gameObject.SetActive(true);
                    interactText.text = "Press E to play RouletteGame";
                }

                // 检测是否按下 E 键
                if (Input.GetKeyDown(KeyCode.E))
                {
                    UnityEngine.Cursor.lockState = CursorLockMode.None;
                    sceneLoader.LoadScene("WheelScene");
                    //Debug.Log("RouletteGame");
                }
                return; // 如果有命中，结束 Update
            }
            else if (tag == "SlotGame")
            {
                currentBox = hit.collider.gameObject;
                // 显示交互提示
                if (interactText != null)
                {
                    interactText.gameObject.SetActive(true);
                    interactText.text = "Buy DLC to play More Game!!!!!";
                }

                // 检测是否按下 E 键
                if (Input.GetKeyDown(KeyCode.E))
                {
                    //AchievementManager.instance.Unlock("Under construction");
                    //UnityEngine.Cursor.lockState = CursorLockMode.None;
                    //sceneLoader.LoadScene("SlotScene");
                    //Debug.Log("SlotGame");
                }
                return; // 如果有命中，结束 Update
            }
        }

        if (Physics.Raycast(transform.position, transform.forward, out hit, interactDistance, npcLayer))
        {
            string tag = hit.collider.tag;

            
            if (tag == "staff")
            {
                currentBox = hit.collider.gameObject;

                if (interactText != null)
                {
                    interactText.gameObject.SetActive(true);
                    interactText.text = "Press E to exchange";
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    EventSystem.Instance.TriggerEvent("Exchange", "callUI", 0);
                    Debug.Log("hi");
                }
                return;
            }
        }

        // 如果没有命中箱子，隐藏交互提示
        currentBox = null;
        if (interactText != null)
        {
            interactText.gameObject.SetActive(false);
        }
    }
}
