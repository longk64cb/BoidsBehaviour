using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponParent : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private SpriteRenderer characterSprite;

    [SerializeField] public Transform weaponRangePosition;
    [SerializeField] private float range = 0.25f;

    private void Awake()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        //AimWeapon(GameManager.Instance.mainCamera.ScreenToWorldPoint(Input.mousePosition));
    }

    public void AimWeapon(Vector3 aimPos)
    {
        Vector2 direction = (Vector2)(aimPos - transform.position).normalized;
        transform.right = direction;

        Vector2 scale = transform.localScale;

        if (direction.x > 0) scale.y = 1f;
        else if (direction.x < 0) scale.y = -1f;

        transform.localScale = scale;

        //if (transform.eulerAngles.z > 225f && transform.eulerAngles.z < 315f)
        //{
        //    sprite.sortingOrder = characterSprite.sortingOrder + 1;
        //}
        //else
        //{
        //    sprite.sortingOrder = characterSprite.sortingOrder - 1;
        //}
    }

    public List<CharacterBehavior> GetAttackableCharacter(LayerMask layer)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(weaponRangePosition.position, range, layer);
        List<CharacterBehavior> characters = new List<CharacterBehavior>();

        foreach (Collider2D collider in colliders)
        {
            if (collider.TryGetComponent<CharacterBehavior>(out CharacterBehavior character)) {
                characters.Add(character);
            }
        }
        return characters;
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawWireSphere(weaponRangePosition.position, range);
    //}
}
