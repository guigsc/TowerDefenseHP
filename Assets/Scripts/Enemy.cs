using Scripts.Enum;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    [SerializeField] TargetFieldPosition targetFieldPosition;
    [SerializeField] protected float speed;
    [SerializeField] private float attackRate;
    [SerializeField] private int attackDamage;
    [SerializeField] private int health;

    private float nextAttack;

    private bool hasReachedPosition;
    private Vector3 targetPosition;

    public UnityEvent<Enemy> onDeath = new UnityEvent<Enemy>();

    private void Start()
    {
        targetPosition = GetTargetPosition();
    }

    private void Update()
    {
        if (!hasReachedPosition)
        {
            Move(targetPosition);
        }
        else
        {
            Attack();
        }
    }

    private void Move(Vector3 position)
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
        if (transform.position.x >= position.x)
        {
            speed = 0;
            hasReachedPosition = true;
        }
    }

    private void Attack()
    {
        if (Time.time > nextAttack)
        {
            Tower.Instance.DealDamage(attackDamage);
            nextAttack = Time.time + attackRate;
        }
    }

    private Vector3 GetTargetPosition()
    {
        var targetField = FindObjectsOfType<TargetField>()
            .Where(t => t.transform.position.x < 0 && t.TargetFieldPosition == targetFieldPosition)
            .FirstOrDefault();
        
        if (targetField != null)
        {
            return targetField.transform.position;
        }

        return Vector3.zero;
    }

    public void DealDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (onDeath != null)
            onDeath.Invoke(this);

        Destroy(gameObject, 0.2f);
    }
}
