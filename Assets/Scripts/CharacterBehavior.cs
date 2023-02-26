using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CharacterBehavior : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private float health = 5f;
    [SerializeField] private float maxHealth = 5f;
    [SerializeField] private float damage = 1f;
    public float Damage => damage;
    public float Health => health;
    public float MaxHealth => maxHealth;

    [SerializeField] private float attackForce = 5f;
    public float AttackForce => attackForce;

    protected bool isDead = false;

    protected Rigidbody2D rgBody;
    protected Animator animator;

    [SerializeField] protected SpriteRenderer characterSprite;
    [SerializeField] private WeaponParent weapon;

    [SerializeField] private Animator weaponAnimator;

    [SerializeField] private float attackDelay = 0.75f;
    [SerializeField] private LayerMask enemyLayer;
    private bool isAttacking = false;

    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material flashMaterial;
    private bool isInjured = false;

    private Vector2 orgSpritePos;

    protected virtual void Start()
    {
        orgSpritePos = characterSprite.transform.localPosition;

        rgBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public virtual void Move(Vector2 velocity, bool isRunning)
    {
        if (/*isDead || */isInjured)
        {
            return;
        }
        rgBody.velocity = (Vector3)velocity * speed;
        animator.SetBool("IsMoving", velocity.magnitude > 0);
        animator.SetBool("IsRunning", isRunning);
    }

    public virtual void Turn(Vector2 position)
    {
        if (isDead) return;

        if (position.x - transform.position.x > 0f) characterSprite.flipX = false;
        else if (position.x - transform.position.x < 0f) characterSprite.flipX = true;

        if (!isAttacking)
            weapon.AimWeapon(position);
    }

    public void Attack()
    {
        if (isAttacking) return;

        weaponAnimator.SetTrigger("Attack");
        isAttacking = true;
        StartCoroutine(AttackCooldown());

        List<CharacterBehavior> characters = weapon.GetAttackableCharacter(enemyLayer);
        if (characters.Contains(this)) characters.Remove(this);
        foreach (CharacterBehavior character in characters)
        {
            character.Injured(this);
            //character.KnockedBack(character.transform.position - transform.position, attackForce);
        }
    }

    private IEnumerator AttackCooldown()
    {
        float timeStamp = 0f;
        while (timeStamp < attackDelay)
        {
            timeStamp += Time.deltaTime;
            yield return null;
        }
        isAttacking = false;
    }

    public virtual void Injured(CharacterBehavior character)
    {
        if (isDead) return;

        health -= character.Damage;

        characterSprite.transform.DOShakePosition(0.15f, 0.3f, 1).OnComplete(() =>
        {
            characterSprite.transform.localPosition = orgSpritePos;
        }) ;
        KnockedBack(transform.position - character.transform.position, character.AttackForce);

        if (health > 0f)
        {
            //Debug.Log(gameObject.name +" Lose " + health);
            animator.SetTrigger("Hurt");
        }
        else
        {
            Die();
            //after death
        }
    }

    protected virtual void Die()
    {
        //Debug.Log(gameObject.name + " die");
        animator.SetTrigger("Die");
        isDead = true;
        //rgBody.Sleep();
        //Move(Vector3.zero, false);
    }

    public void KnockedBack(Vector2 direction, float strength)
    {
        isInjured = true;
        direction.Normalize();
        rgBody.velocity = Vector2.zero;
        rgBody.AddForce(direction * strength, ForceMode2D.Impulse);
        characterSprite.material = flashMaterial;
        StartCoroutine(KnockBackReset());
    }

    private IEnumerator KnockBackReset()
    {
        yield return ExtensionClass.GetWaitForSeconds(0.3f);
        rgBody.velocity = Vector2.zero;
        characterSprite.material = defaultMaterial;
        //yield return ExtensionClass.GetWaitForSeconds(1f);
        isInjured = false;
    }
}
