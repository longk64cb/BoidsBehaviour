using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Image heartPrefab;

    private List<Image> heartBarList = new List<Image>();

    private void Start()
    {
        InitHealthBar();
        UpdateHealthBar();
    }

    private void OnEnable()
    {
        EventDispatcher.Instance.RegisterListener(EventID.OnUpdateHealth, UpdateHealthBar);
    }

    private void OnDisable()
    {
        EventDispatcher.Instance.RemoveListener(EventID.OnUpdateHealth, UpdateHealthBar);
    }

    private void InitHealthBar()
    {
        float maxHealth = /*GameManager.Instance.Player.MaxHealth; */5;
        for (int i = 0; i < maxHealth; i++)
        {
            Image heart = Instantiate(heartPrefab, transform);
            heartBarList.Add(heart);
        }
    }

    private void UpdateHealthBar(object param = null)
    {
        float health = GameManager.Instance.Player.Health;
        Debug.Log(health);
        for (int i = 0; i < heartBarList.Count; i++)
        {
            heartBarList[i].gameObject.SetActive(i <= health);
            heartBarList[i].fillAmount = 1f;
        }

        int roundedHealth = (int)Mathf.Floor(health);
        float amount = health - roundedHealth;
        if (amount > 0) heartBarList[roundedHealth].fillAmount = amount;
    }
}
