using System.Collections;
using UnityEngine;

namespace UnityEngine
{
    public class DiceGameEventInitializer : MonoBehaviour
    {
        // Singleton pattern for GameRoot (do not touch)
        private static DiceGameEventInitializer _instance;
        public static DiceGameEventInitializer Instance { get { return _instance; } }

        [Header("Game Objects")]
        public GameObject Cup;
        public GameObject DicePrefab;
        public GameObject mainCam;

        [Header("Position Transforms")]
        public Transform positionUP;
        public Transform positionDOWN;
        public Transform positionStart;
        public Transform positionEnd;

        private GameObject dice1;
        private GameObject dice2;
        private bool spawning = false;

        [Header("Settings")]
        public float duration = 1.0f; // Duration for smooth movement

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            RegisterEvents();
        }

        #region Event Registration
        private void RegisterEvents()
        {
            EventSystem.Instance.RegisterEvent<string>("DiceGameEvent", "RollDice", RollingDice);
            EventSystem.Instance.RegisterEvent<int>("DiceGameEvent", "MoveCup", MoveCup);
            EventSystem.Instance.RegisterEvent<int>("DiceGameEvent", "MoveCamera", MoveCamera);
            EventSystem.Instance.RegisterEvent<int>("DiceGameEvent", "SpawnDice", SpawnDice);
            EventSystem.Instance.RegisterEvent<int>("DiceGameEvent", "StopSpawn", StopSpawn);
        }
        #endregion

        #region Event Handlers
        private void RollingDice(string p_DiceCase)
        {
            dice1 = GameObject.Find("dice1");
            dice2 = GameObject.Find("dice2");
            if (!dice1 || !dice2)
            {
                Debug.LogError("Dice objects are not assigned or instantiated.");
                return;
            }

            Debug.Log("Rolling Dice 1 & 2");
            string[] parts = p_DiceCase.Split(' ');
            int dice1Value = int.Parse(parts[0]);
            int dice2Value = int.Parse(parts[1]);
            RotateDice(dice1.transform, dice1Value);
            RotateDice(dice2.transform, dice2Value);
        }

        private void MoveCamera(int p_Options)
        {
            if (!mainCam)
            {
                Debug.LogError("Main Camera is not assigned.");
                return;
            }

            if (p_Options == 0)
            {
                mainCam.transform.position = positionStart.position;
                mainCam.transform.rotation = positionStart.rotation;
            }
            else if (p_Options == 1)
            {
                mainCam.transform.position = positionEnd.position;
                mainCam.transform.rotation = positionEnd.rotation;
            }
        }

        private void MoveCup(int p_Options)
        {
            Cup = GameObject.Find("dice_cup");
            positionUP = GameObject.Find("UP").transform;
            positionDOWN = GameObject.Find("Down").transform;
            positionStart = GameObject.Find("Cam_start").transform;
            positionEnd = GameObject.Find("Cam_end").transform;
            if (!Cup || !positionUP || !positionDOWN)
            {
                Debug.LogError("Cup or position transforms are not assigned.");
                return;
            }

            if (p_Options == 1)
            {
                StartCoroutine(SmoothMove(Cup.transform, positionUP.position));
            }
            else if (p_Options == 0)
            {
                StartCoroutine(SmoothMove(Cup.transform, positionDOWN.position));
            }
        }

        private void SpawnDice(int p_lifeTime)
        {
            if (!DicePrefab || !positionDOWN)
            {
                Debug.LogError("DicePrefab or positionDOWN is not assigned.");
                return;
            }

            spawning = true;
            StartCoroutine(SpawnDiceCoroutine(p_lifeTime));
        }

        private void StopSpawn(int _)
        {
            spawning = false;
        }
        #endregion

        #region Helper Functions
        private IEnumerator SmoothMove(Transform objectToMove, Vector3 targetPosition)
        {
            Vector3 startPosition = objectToMove.position;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                objectToMove.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            objectToMove.position = targetPosition;
        }

        private void RotateDice(Transform diceTransform, int faceValue)
        {
            Quaternion targetRotation = faceValue switch
            {
                1 => Quaternion.Euler(0, 0, 0),
                2 => Quaternion.Euler(0, 0, 90),
                3 => Quaternion.Euler(90, 0, 0),
                4 => Quaternion.Euler(-90, 0, 0),
                5 => Quaternion.Euler(0, 0, -90),
                6 => Quaternion.Euler(180, 0, 0),
                _ => Quaternion.identity
            };

            Debug.Log($"Rotating {diceTransform.name} to face {faceValue}");
            diceTransform.rotation = targetRotation;
        }

        private IEnumerator SpawnDiceCoroutine(int p_lifeTime)
        {
            while (spawning)
            {
                GameObject newDice = Instantiate(DicePrefab, positionDOWN.position, Quaternion.identity);
                newDice.name = "Dice";
                int randomFace = Random.Range(1, 7);
                RotateDice(newDice.transform, randomFace);
                Destroy(newDice, p_lifeTime);
                Debug.Log($"Spawned Dice with Face {randomFace}. It will be destroyed in {p_lifeTime} seconds.");
                yield return new WaitForSeconds(0.01f);
            }
        }
        #endregion
    }
}
