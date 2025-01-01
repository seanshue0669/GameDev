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
            Debug.Log("Rolling Dice 1&2");
            var result = AnalyisCase(p_DiceCase);
            if (result.HasValue) 
            {
                var (dice1Value, dice2Value) = result.Value;
                RotateDice(Dice1.transform, dice1Value);
                RotateDice(Dice2.transform, dice2Value);
            }
            else
            {
                Debug.LogError("Invalid Dice Case format");
            }
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
        private (int, int)? AnalyisCase(string p_Case)
        {
            string[] parts = p_Case.Split(' ');
            if (int.TryParse(parts[0], out int dice1Value) &&int.TryParse(parts[1], out int dice2Value))
            {
                return (dice1Value, dice2Value);
            }
            else
                return null;
        }
        private void RotateDice(Transform diceTransform, int faceValue)
        {
            // Define rotation for each face
            Quaternion targetRotation = Quaternion.identity;
            switch (faceValue)
            {
                case 1:
                    targetRotation = Quaternion.Euler(0, 0, 0); // 1 facing up
                    break;
                case 2:
                    targetRotation = Quaternion.Euler(0, 0, 90); // 2 facing up
                    break;
                case 3:
                    targetRotation = Quaternion.Euler(90, 0, 0); // 3 facing up
                    break;
                case 4:
                    targetRotation = Quaternion.Euler(-90, 0, 0); // 4 facing up
                    break;
                case 5:
                    targetRotation = Quaternion.Euler(0, 0, -90); // 5 facing up
                    break;
                case 6:
                    targetRotation = Quaternion.Euler(180, 0, 0); // 6 facing up
                    break;
                default:
                    //Should be modify
                    targetRotation = Quaternion.Euler(0, 0, 0); // 1 facing up
                    break;
            }
            diceTransform.rotation = targetRotation;
        }
    }
}
