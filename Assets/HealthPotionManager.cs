using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthPotionManager : MonoBehaviour
{
    [SerializeField] private int maxCount = 5;
    private int currentCount;
    [SerializeField] private int healAmount = 35;
    private HealthManager _health;
    public Text currentCountText; // UI용

    void Awake()
    {
        _health = GetComponent<HealthManager>();
    }

    void Start()
    {
        FullReplenishPotion();
    }

    public void AddMaxCount(int count)
    {
        maxCount += count;
    }

    public void AddCurrentCount(int count)
    {
        currentCount += count;
        currentCountText.text = currentCount.ToString();

        if(currentCount > maxCount)
        {
            currentCount = maxCount;
        }
    }

    public void DecreaseCurrentCount(int count)
    {
        currentCount -= count;
        currentCountText.text = currentCount.ToString();

        if(currentCount < 0)
        {
            currentCount = 0;
        }
    }

    public int GetMaxCount()
    {
        return maxCount;
    }

    public int GetCurrentCount()
    {
        return currentCount;
    }

    public void UseHealthPotion()
    {
        if(currentCount < 0) { return; }

        _health.AddCurrentParam(healAmount);
        DecreaseCurrentCount(1);
    }

    public void FullReplenishPotion()
    {
        currentCount = maxCount;
        currentCountText.text = currentCount.ToString();
    }
}
