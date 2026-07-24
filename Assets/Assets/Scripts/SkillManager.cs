using System.Collections;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [Header("Script References")]
    public Movement playerMovement;
    public Timer gameTimer;

    [Header("Skill B: Time Freeze")]
    public float timeFreezeDuration = 5f;
    public TimeFreezeEffect timeFreezeEffect;

    [Header("Skill C: Speed Boost")]
    public float speedBoostMultiplier = 1.5f; // e.g., 1.5 = 50% increase
    public float speedBoostDuration = 5f;
    private float baseSpeed;
    private int activeSpeedBoosts = 0;

    private void Start()
    {
        // Automatically grab the Movement script attached to the player
        if (playerMovement == null)
        {
            playerMovement = GetComponent<Movement>();
        }
        // Store the base speed for proper revert after stacking
        if (playerMovement != null)
        {
            baseSpeed = playerMovement.moveSpeed;
        }
    }

    // This is the main method that Player.cs will trigger
    // Returns true if the skill was activated, false if not implemented
    public bool ActivateSkill(ItemScript.ItemType skillType)
    {
        switch (skillType)
        {
            case ItemScript.ItemType.SkillB:
                StartCoroutine(TimeFreezeRoutine());
                return true;
            case ItemScript.ItemType.SkillC:
                StartCoroutine(SpeedBoostRoutine());
                return true;
            default:
                Debug.Log($"{skillType} is not fully implemented yet.");
                return false;
        }
    }

    private IEnumerator TimeFreezeRoutine()
    {
        if (gameTimer != null)
        {
            Debug.Log("Skill B Activated: Time Frozen!");
            gameTimer.isTimeFrozen = true;
            
            if (timeFreezeEffect != null)
                timeFreezeEffect.SetEffectActive(true);

            // Wait for the duration
            yield return new WaitForSeconds(timeFreezeDuration);

            gameTimer.isTimeFrozen = false;
            
            if (timeFreezeEffect != null)
                timeFreezeEffect.SetEffectActive(false);
                
            Debug.Log("Skill B Ended: Time Resumed.");
        }
    }

    private IEnumerator SpeedBoostRoutine()
    {
        if (playerMovement != null)
        {
            activeSpeedBoosts++;
            Debug.Log($"Skill C Activated: Speed Boosted! (x{activeSpeedBoosts} stacked)");

            // Apply stacked multiplier based on base speed
            playerMovement.moveSpeed = baseSpeed * Mathf.Pow(speedBoostMultiplier, activeSpeedBoosts);

            // Wait for the duration
            yield return new WaitForSeconds(speedBoostDuration);

            activeSpeedBoosts--;
            if (activeSpeedBoosts > 0)
            {
                // Still has active boosts, recalculate
                playerMovement.moveSpeed = baseSpeed * Mathf.Pow(speedBoostMultiplier, activeSpeedBoosts);
            }
            else
            {
                // All boosts expired, revert to base speed
                playerMovement.moveSpeed = baseSpeed;
            }
            Debug.Log($"Skill C Ended. Speed: {playerMovement.moveSpeed}");
        }
    }
}