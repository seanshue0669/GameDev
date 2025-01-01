using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace UnityEngine
{
    public class DiceGameEventInitiler : MonoBehaviour
    {
        [SerializeField]
        // Gambling objects
        public GameObject Cup;
        private GameObject Dice1;
        private GameObject Dice2;
        public GameObject mainCam;
        // Position objects
        [SerializeField] 
        public Transform postionUP;
        public Transform postionDOWN;
        public Transform postitionStart;
        public Transform postitionEnd;

        // Duration for smooth movement
        [SerializeField] public float duration = 1.0f;


        void Awake()
        {
            EventSystem.Instance.RegisterEvent<string>("DiceGameEvent", "RollDice", RollingDice);
            EventSystem.Instance.RegisterEvent<int>("DiceGameEvent", "MoveCup", MoveCup);
            EventSystem.Instance.RegisterEvent<int>("DiceGameEvent", "MoveCamera", MoveCamera);
            //EventSystem.Instance.RegisterEvent<int>("DiceGameEvent", "SpwanDice", SpwanDice());
            Debug.Log("Reg Rolling Event");
        }
        #region register function
        void RollingDice(string p_DiceCase)
        {
            // Implementation for RollingDice
            Dice1 = GameObject.Find("dice1");
            Dice2 = GameObject.Find("dice2");
            Debug.Log("Rolling Dice 1&2");
            string[] parts = p_DiceCase.Split(' ');
            int dice1Value = int.Parse(parts[0]);
            int dice2Value = int.Parse(parts[1]);
            RotateDice(Dice1.transform, dice1Value);
            RotateDice(Dice2.transform, dice2Value);
        }
        void MoveCamera(int p_Options)
        {
            if (p_Options == 0)
            {
                mainCam.transform.position = postitionStart.position;
                mainCam.transform.rotation = postitionStart.rotation;
            }
            else if (p_Options == 1)
            {
                mainCam.transform.position = postitionEnd.position;
                mainCam.transform.rotation = postitionEnd.rotation;
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
        void SpawnDice(int p_lifeTime)
        {

        }
        #endregion


        #region support fuction
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
        
        private void RotateDice(Transform diceTransform, int faceValue)
        {
            Quaternion targetRotation = Quaternion.identity;
            switch (faceValue)
            {
                case 0: targetRotation = Quaternion.Euler(45, 0, 45);break;
                case 1: targetRotation = Quaternion.Euler(0, 0, 0); break;
                case 2: targetRotation = Quaternion.Euler(0, 0, 90); break;
                case 3: targetRotation = Quaternion.Euler(90, 0, 0); break;
                case 4: targetRotation = Quaternion.Euler(-90, 0, 0); break;
                case 5: targetRotation = Quaternion.Euler(0, 0, -90); break;
                case 6: targetRotation = Quaternion.Euler(180, 0, 0); break;
                //Special Case
                case 7: targetRotation = Quaternion.Euler(0, 0, 0); break;
                default: targetRotation = Quaternion.Euler(0, 0, 0); break;
            }
            Debug.Log($"Rotating {diceTransform.name} to face {faceValue}");
            diceTransform.rotation = targetRotation;
        }
        #endregion
    }
}
