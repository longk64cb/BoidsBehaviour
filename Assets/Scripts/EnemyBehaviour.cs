using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyBehaviour : CharacterBehavior
{
    private EnemyAI AI;

    protected override void Start()
    {
        base.Start();
        AI = GetComponent<EnemyAI>();
    }

    protected override void Die()
    {
        AI.StopAllCoroutines();
        AI.canMove = false;
        //AI.enabled = false;
        base.Die();
        StartCoroutine(DropItemAndDisappear());
    }

    private IEnumerator DropItemAndDisappear()
    {
        yield return new WaitForSeconds(1f);
        DropItem();
        Destroy(gameObject);
    }

    private void DropItem()
    {

    }

    public void BeginAttack()
    {
        float orgPosY = characterSprite.transform.localPosition.y;
        characterSprite.transform.DOLocalMoveY(orgPosY + 0.5f, 0.2f).OnComplete(() =>
        {
            characterSprite.transform.DOLocalMoveY(orgPosY, 0.2f);
        });
    }
}
