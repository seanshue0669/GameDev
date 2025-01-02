using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace UnityEngine
{
    public class DiceGameEvent : MonoBehaviour
    {
        private static DiceGameEvent instance;

        public static DiceGameEvent Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DiceGameEvent();
                }
                return instance;
            }
        }
        [SerializeField]
        // Gambling objects
        public GameObject Cup;
        public GameObject DicePrefab;
        private GameObject Dice1;
        private GameObject Dice2;
        public GameObject mainCam;
        // Position objects
        [SerializeField] 
        public Transform postionUP;
        public Transform postionDOWN;
        public Transform postitionStart;
        public Transform postitionEnd;

        private bool spwaning= false;

        public bool isInit = false;
        // Duration for smooth movement
        [SerializeField] public float duration = 1.0f;


        public void InitEvent()
        {
            EventSystem.Instance.RegisterEvent<int>("DiceGameEvent", "FindObject", FindObject);
            EventSystem.Instance.RegisterEvent<string>("DiceGameEvent", "RollDice", RollingDice);
            EventSystem.Instance.RegisterEvent<int>("DiceGameEvent", "MoveCup", MoveCup);
            EventSystem.Instance.RegisterEvent<int>("DiceGameEvent", "MoveCamera", MoveCamera);
            EventSystem.Instance.RegisterEvent<int>("DiceGameEvent", "SpawnDice", SpawnDice);
            EventSystem.Instance.RegisterEvent<int>("DiceGameEvent", "StopSpawn", StopSpawn);
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
            spwaning = true;
            StartCoroutine(SpawnDiceCoroutine(p_lifeTime));
        }       
        #endregion
        
        void FindObject()
        {
            mainCam = GameObject.Find("Camera");
            Cup = GameObject.Find("dice_cup");

            postionUP = GameObject.Find("UP").transform;
            postionDOWN = GameObject.Find("Down").transform;
            postitionStart = GameObject.Find("Cam_start").transform;
            postitionEnd = GameObject.Find("Cam_end").transform;

        }

        #region support fuction
        private void FindObject(int p)
        {
            mainCam = GameObject.Find("Camera");
            Cup = GameObject.Find("dice_cup");

            postionUP = GameObject.Find("UP").transform;
            postionDOWN = GameObject.Find("Down").transform;
            postitionStart = GameObject.Find("Cam_start").transform;
            postitionEnd = GameObject.Find("Cam_end").transform;

        }
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
                case 7: targetRotation = Quaternion.Euler(45, 0, 45); break;

                default: targetRotation = Quaternion.Euler(0, 0, 0); break;
            }
            Debug.Log($"Rotating {diceTransform.name} to face {faceValue}");
            diceTransform.rotation = targetRotation;
        }
        private IEnumerator SpawnDiceCoroutine(int p_lifeTime)
        {
            while (spwaning)
            {
                GameObject newDice = Instantiate(DicePrefab, postionDOWN.position, Quaternion.identity);
                newDice.name = "Dice";
                newDice.transform.position = postionDOWN.position;
                int randomFace = Random.Range(1, 7);
                RotateDice(newDice.transform, randomFace);
                Destroy(newDice, p_lifeTime);
                Debug.Log($"Spawned Dice with Face {randomFace}. It will be destroyed in {p_lifeTime} seconds.");
                yield return new WaitForSeconds(0.01f);
            }
        }
        private void StopSpawn(int _p)
        {
            spwaning = false;
        }
        #endregion
    }
}