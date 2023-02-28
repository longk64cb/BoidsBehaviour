using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Collectable : PooledObject
{
    [SerializeField] CollectableType type;
    public CollectableType Type => type;

    [SerializeField] SpriteRenderer sprite;
    [SerializeField] float bounceTime = 1f;
    [SerializeField] float speed = 2f;

    private bool isCollected = false;

    private void OnEnable()
    {
        isCollected = false;
    }

    [ContextMenu("Spawn")]
    public void Spawn()
    {
        Vector3 endPos = (Vector2) transform.position + Random.insideUnitCircle * 1f;
        sprite.transform.DOLocalJump(sprite.transform.localPosition, 1f, 1, 0.5f);
        transform.DOMove(endPos, 0.5f).OnComplete(() =>
        {
            StartCoroutine(Bouncing());
        });
    }

    private IEnumerator Bouncing()
    {
        Vector2 direction = Random.value > 0.5f ? Vector2.up : Vector2.down;
        float timeStamp = Random.Range(0f, bounceTime);
        while (true)
        {
            sprite.transform.localPosition = Vector2.Lerp(sprite.transform.localPosition, (Vector2) sprite.transform.localPosition + direction, Time.deltaTime * speed);
            timeStamp += Time.deltaTime;
            if (timeStamp >= bounceTime)
            {
                timeStamp = 0f;
                direction = direction == Vector2.up ? Vector2.down : Vector2.up;
            }
            yield return null;
        }
    }

    public void Collected()
    {
        isCollected = true;
        StopCoroutine(Bouncing());
        StartCoroutine(FlyToPlayer());
    }

    private IEnumerator FlyToPlayer()
    {
        PlayerBehavior player = GameManager.Instance.Player;
        while (Vector2.SqrMagnitude(player.transform.position - transform.position) > 0.25f)
        {
            transform.position = Vector2.Lerp(transform.position, player.transform.position, Time.deltaTime * 8f);
            yield return null;
        }
        player.Collect(this);
        ReturnToPool();
    }
}
