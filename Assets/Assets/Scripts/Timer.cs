using System;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float remainingTime;

    public GameManagerScript gameManager;
    private bool isDead;

    // NEW: Public boolean for the SkillManager to control
    public bool isTimeFrozen = false;

    void Update()
    {
        // NEW: Only count down if time is NOT frozen
        if (!isTimeFrozen)
        {
            if (remainingTime > 0)
            {
                remainingTime -= Time.deltaTime;
            }
            else if (remainingTime <= 0 && !isDead)
            {
                remainingTime = 0;
                timerText.color = Color.red;

                isDead = true;
                gameManager.GameOver();
                Debug.Log("Dead");
            }
        }

        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}