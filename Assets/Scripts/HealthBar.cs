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
        float maxHealth = GameManager.Instance.Player.MaxHealth;
        for (int i = 0; i < maxHealth; i++)
        {
            Image heart = Instantiate(heartPrefab, transform);
            heartBarList.Add(heart);
        }
    }

    private void UpdateHealthBar(object param = null)
    {
        float health = GameManager.Instance.Player.Health;
        for (int i = 0; i < heartBarList.Count; i++)
        {
            heartBarList[i].gameObject.SetActive(i <= health);
        }

        int roundedHealth = (int)Mathf.Floor(health);
        float amount = health - roundedHealth;
        heartBarList[roundedHealth].fillAmount = amount;
    }
}
