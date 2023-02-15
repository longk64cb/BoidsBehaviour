using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : CharacterBehavior
{
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
}
