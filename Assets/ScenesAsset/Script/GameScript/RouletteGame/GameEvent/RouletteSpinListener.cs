using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class RouletteSpinListener : MonoBehaviour
{
    private static rouletteballthrowout _instance;
    public static rouletteballthrowout Instance => _instance ??= new rouletteballthrowout();

    private Transform rouletteWheel;  // 轉盤本體
    private Transform rouletteBall;   // 球的中心父物件
    private Rigidbody rb;             // 球本身的 Rigidbody

    private bool isSpinning = false;

    // 假設輪盤 Z=0 時，各號碼對應的角度（從上往下看，逆時針增長）
    private Dictionary<int, float> numberAngleMap = new Dictionary<int, float>()
    {
        { 0, 0f },
        { 32, 9.73f },
        { 15, 19.46f },
        { 19, 29.19f },
        { 4, 38.92f },
        { 21, 48.65f },
        { 2, 58.38f },
        { 25, 68.11f },
        { 17, 77.84f },
        { 34, 87.57f },
        { 6, 97.3f },
        { 27, 107.03f },
        { 13, 116.76f },
        { 36, 126.49f },
        { 11, 136.22f },
        { 30, 145.95f },
        { 8, 155.68f },
        { 23, 165.41f },
        { 10, 175.14f },
        { 5, 184.87f },
        { 24, 194.6f },
        { 16, 204.33f },
        { 33, 214.06f },
        { 1, 223.79f },
        { 20, 233.52f },
        { 14, 243.25f },
        { 31, 252.98f },
        { 9, 262.71f },
        { 22, 272.44f },
        { 18, 282.17f },
        { 29, 291.9f },
        { 7, 301.63f },
        { 28, 311.36f },
        { 12, 321.09f },
        { 35, 330.82f },
        { 3, 340.55f },
        { 26, 350.28f }
    };

    // 球的初始角度（當輪盤 Z=0 時，球位於 8 號角度 155.68f）
    private float ballPositionAngle = 155.68f;

    // 追蹤累計的 Y 軸旋轉角度
    private float totalBallY = 0f;

    private void Start()
    {
        // 註冊自訂事件
        EventSystem.Instance.RegisterEvent<int>("Roulette", "spin", Spinning);
    }

    private void Spinning(int winningNumber)
    {
        rouletteWheel = GameObject.Find("Roulette.002").transform;
        rouletteBall = GameObject.Find("ballcenter").transform;

        rb = GameObject.Find("ball").GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Ball 沒有 Rigidbody 組件");
        }

        // 一開始為了「假物理」，關掉球重力
        rb.isKinematic = true;
        rb.useGravity = false;

        // 初始化 totalBallY 為球的初始 Y 角度
        totalBallY = rouletteBall.localEulerAngles.y;
        if (isSpinning) return;
        isSpinning = true;

        // 重置球的位置
        GameObject.Find("ball").transform.localPosition = new Vector3(0.294522166f, -0.127312273f, -0.652947962f);
        rouletteWheel.transform.localEulerAngles = new Vector3(-90f,0f,0f);
        rouletteBall.transform.localEulerAngles = Vector3.zero;

        // Step1: 假旋轉 4 秒；Step2: 只轉球 0.5 秒
        StartCoroutine(SpinningCoroutine(4f, winningNumber));
    }

    private IEnumerator SpinningCoroutine(float spinDuration, int winningNumber)
    {
        // ===========================
        // (1) 假旋轉階段 - CCW only
        // ===========================
        float spinSpeed = 360f; // 初始旋轉速度（度/秒）
        float ballSpeed = 480f; // 初始旋轉速度（度/秒）
        float deceleration = spinSpeed / spinDuration; // 每秒減速量
        float currentTime = 0f;

        // 確保球的物理屬性為 kinematic 且不受重力影響
        rb.isKinematic = true;
        rb.useGravity = false;

        totalBallY = 0;
        while (currentTime < spinDuration)
        {
            rouletteWheel.Rotate(0, 0, -spinSpeed * Time.deltaTime);
            rouletteBall.Rotate(0, ballSpeed * Time.deltaTime, 0);

            // 更新累計旋轉角度
            totalBallY += ballSpeed * Time.deltaTime;

            // 每幀減速
            spinSpeed = Mathf.Max(0f, spinSpeed - deceleration * Time.deltaTime);
            ballSpeed = Mathf.Max(180f, ballSpeed - deceleration * Time.deltaTime);

            currentTime += Time.deltaTime;
            yield return null;
        }

        totalBallY += ballSpeed * Time.deltaTime;

        float finalAngleWheel = rouletteWheel.localEulerAngles.z;

        // ===========================
        // (2) 只旋轉球 - CCW only，使用自訂角度插值
        // ===========================
        float rotateDuration = 1.0f; // 旋轉持續時間（秒）
        float elapsed = 0f;

        // 從字典拿到 "winningNumber" 所對應的角度
        float winningNumberAngle = 0f;
        if (numberAngleMap.ContainsKey(winningNumber))
        {
            // 計算目標角度，調整相對於 8 號的位置
            winningNumberAngle = numberAngleMap[winningNumber] - numberAngleMap[8];
            // 確保角度在 [0, 360) 範圍內
            winningNumberAngle = (winningNumberAngle % 360 + 360) % 360;
        }
        else
        {
            Debug.LogWarning($"字典中沒找到 {winningNumber}，預設 0 度");
            winningNumberAngle = 0f;
        }

        // 取得球目前的 Y 軸角度
        float startBallY = totalBallY;

        // 計算「要讓球 CCW 轉到 winningNumberAngle」的角度差
        float rawDiff = winningNumberAngle - (startBallY % 360f);
        if (rawDiff <= 0f)
        {
            rawDiff += 360f; // 確保是正向 (CCW)
        }

        // 最終 Y 角度 = 起始 + 正向差
        float endBallY = startBallY + rawDiff;

        while (elapsed <= rotateDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / rotateDuration);

            // 計算目前的總 Y 角度
            float currentTotalBallY = Mathf.Lerp(startBallY, endBallY, t);

            // 應用到球的旋轉
            rouletteBall.localEulerAngles = new Vector3(
                rouletteBall.localEulerAngles.x,
                currentTotalBallY % 360f,
                rouletteBall.localEulerAngles.z
            );

            yield return null;
        }
        /*
        // 收尾：把球角度強制到位
        rouletteBall.localEulerAngles = new Vector3(
            rouletteBall.localEulerAngles.x,
            endBallY % 360f,
            rouletteBall.localEulerAngles.z
        );
        */
        // 更新累計旋轉角度
        totalBallY = endBallY;

        // ===========================
        // (3) 開啟球的重力
        // ===========================
        rb.isKinematic = false;
        rb.useGravity = true;

        isSpinning = false;
    }

    // 若需要可保留此函式計算落點，但本需求只在 Step2 做球旋轉，不使用位置插值
    private Vector3 CalculateBallPositionByNumber(int number)
    {
        float rawAngle = 0f;
        if (numberAngleMap.ContainsKey(number))
            rawAngle = numberAngleMap[number];

        float angleRad = rawAngle * Mathf.Deg2Rad;
        float radius = 0.3f;
        float x = radius * Mathf.Cos(angleRad);
        float z = radius * Mathf.Sin(angleRad);

        return new Vector3(x, 0.12f, z);
    }
}