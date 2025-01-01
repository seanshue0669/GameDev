using System.Collections;
using UnityEngine;

namespace UnityEngine
{
    public class DiceGameEventInitiler : MonoBehaviour
    {
        [SerializeField]
        // Gambling objects
        public GameObject Dice1;
        public GameObject Dice2;
        public GameObject Cup;

        // Position objects
        [SerializeField] public Transform postionUP;
        [SerializeField] public Transform postionDOWN;

        // Duration for smooth movement
        [SerializeField] public float duration = 1.0f;

        void Awake()
        {
            EventSystem.Instance.RegisterEvent<string>("DiceGameEvent", "RollDice", RollingDice);
            EventSystem.Instance.RegisterEvent<int>("DiceGameEvent", "MoveCup", MoveCup);
            Debug.Log("Reg Rolling Event");
        }

        void RollingDice(string p_DiceCase)
        {
            // Implementation for RollingDice

        }

        void MoveCup(int p_Options)
        {
            if (p_Options == 1)
            {
                StartCoroutine(SmoothMove(Cup.transform, postionUP.position));
            }
            else if (p_Options == 0)
            {
                StartCoroutine(SmoothMove(Cup.transform, postionDOWN.position));
            }
        }
        // Tool Function
        private IEnumerator SmoothMove(Transform p_objectToMove, Vector3 p_targetPosition)
        {
            Vector3 startPosition = p_objectToMove.position;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                p_objectToMove.position = Vector3.Lerp(startPosition, p_targetPosition, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            // Ensure the object reaches the target position
            p_objectToMove.position = p_targetPosition;
        }
    }
}
