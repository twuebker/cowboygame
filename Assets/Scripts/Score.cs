using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    
    public Text pointsText;
    public TextMeshProUGUI text;
    private static int score;

    private static Score instance;
    public static Score Instance {
        get {
            if (instance == null) {
                Debug.Log("Score instance is null!");
            }
            return instance;
        }
    }

    void Awake() 
    {
        instance = this;
    }
    void Update()
    {
        text.SetText("Score: " + score.ToString());
    }

    public static void AddScore(int scoreToAdd) {
        score += scoreToAdd;
    }

    public static void SetScore(int scoreToSet) {
        score = scoreToSet;
    }

    public void Setup(int score)
    {
        gameObject.SetActive(true);
        pointsText.text = score.ToString + " POINTS";
    }
}
