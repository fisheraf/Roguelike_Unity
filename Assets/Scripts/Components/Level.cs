using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public int currentLevel;
    public int currentXP;
    public int levelUpBase;
    public int levelUpFactor;

    LevelUp levelUp = null;

    private void Start()
    {
        levelUp = FindObjectOfType<LevelUp>();
    }

    public int experienceToNextLevel()
    {
        return levelUpBase + (currentLevel * levelUpFactor);
    }

    public void addXP(int XP)
    {
        currentXP += XP;

        if (currentXP > experienceToNextLevel())
        {
            currentXP -= experienceToNextLevel();
            currentLevel++;
            FindObjectOfType<UIManager>().NewMessage("You feel stronger! You reached level: " + currentLevel);
            levelUp.OpenMenu();
        }
    }

}
