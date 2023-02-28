using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : CharacterBehavior
{
    [SerializeField] private int coin = 0;
    // Update is called once per frame
    void Update()
    {
        if (isDead) return;

        float horz = Input.GetAxisRaw("Horizontal");
        float vert = Input.GetAxisRaw("Vertical");

        Move(new Vector2(horz, vert), true);
        Turn(GameManager.Instance.mainCamera.ScreenToWorldPoint(Input.mousePosition));

        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }
    }

    public override void Move(Vector2 velocity, bool isRunning)
    {
        base.Move(velocity, isRunning);
    }

    public override void Injured(CharacterBehavior character)
    {
        base.Injured(character);
    }

    public override void UpdateHealth(float amount)
    {
        base.UpdateHealth(amount);
        EventDispatcher.Instance.PostEvent(EventID.OnUpdateHealth);
    }

    public void UpdateCoin(int amount)
    {
        coin += amount;
        coin = Mathf.Clamp(coin, 0, coin);
    }

    public void Collect(Collectable item)
    {
        switch (item.Type)
        {
            case CollectableType.Heart:
                UpdateHealth(1f);
                break;
            case CollectableType.Coin:
                UpdateCoin(1);
                break;
        }
    }
}
