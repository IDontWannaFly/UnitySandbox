using UnityEngine;
using System.Collections;

public class Bandit : MonoBehaviour, EnemyDetector.EnemyDetectorCallback {

    public float checkDelay = 5f;
    public float attackDelay = 3f;
    public float speed = 5f;
    public LayerMask enemyLayer;
    public float health;
    public EnemyDetector m_detector;
    public float attackRange = 5f;
    public Transform attackPoint;
    public float damage = 20;

    private Rigidbody2D             m_body;
    private Animator                m_animator;
    private State                   state;
    private float                   m_attackDelay = 0f;
    private float                   m_checkDelay = 0f;
    private float                   m_direction = 1;
    private float                   m_health;
    private Transform               m_objectToChase;

    // Use this for initialization
    void Start () {
        m_body = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();
        if(m_detector != null)
            m_detector.attackCallback(this);
        state = State.IDLE;
        m_checkDelay = checkDelay;
        m_health = health;
    }
	
	// Update is called once per frame
	void Update () {
        switch(state){
            case State.CHASE:
                Run();
                break;
            case State.COMBAT:
                Hit();
                break;
            case State.IDLE:
                LookForEnemies();
                break;
        }
        UpdateAnimator();
    }

    private void UpdateAnimator(){
        switch(state){
            case State.COMBAT:
                m_animator.SetInteger("AnimState", 1);
                break;
            case State.IDLE: 
                m_animator.SetInteger("AnimState", 1);
                break;
            case State.CHASE:
                m_animator.SetInteger("AnimState", 2);
                break;
        }
    }

    private void CheckEnemies(){
        m_objectToChase = null;
        m_body.velocity = Vector2.zero;
        m_checkDelay = checkDelay;
        state = State.IDLE;
    }

    private void ChaseEnemy(Transform enemy){
        state = State.CHASE;
        m_objectToChase = enemy;
    }

    private void LookForEnemies(){
        m_checkDelay -= Time.deltaTime;
        if(m_checkDelay <= 0){
            Flip();
            m_checkDelay = checkDelay;
        }
    }

    private void StopAndHit(){
        m_attackDelay = 0.5f;
        m_body.velocity = Vector2.zero;
        state = State.COMBAT;
    }

    private void Hit(){
        if(Vector2.Distance(m_objectToChase.position, transform.position) > attackRange * 4){
            ChaseEnemy(m_objectToChase);
            return;
        }
        m_attackDelay -= Time.deltaTime;
        if(m_attackDelay <= 0f){
            m_attackDelay = attackDelay;
            m_animator.SetTrigger("Attack");
            Invoke("DealDamage", 0.5f);
        }
    }

    private void DealDamage(){
        if(state != State.COMBAT)
            return;
        Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        foreach(var enemy in enemies){
            enemy.GetComponent<PlayerController>().Hurt(damage);
        }
    }

    private void Run(){
        m_body.velocity = transform.right * m_direction * -1 * speed;
        if(m_objectToChase != null && Vector2.Distance(m_objectToChase.position, transform.position) <= attackRange * 2){
            StopAndHit();
        }
    }

    public void Hurt(float damage){
        m_health -= damage;
        m_animator.SetTrigger("Hurt");
        if(m_health <= 0){
            Die();
        }
    }

    private void Die(){
        m_animator.SetBool("IsDead", true);

        m_detector.enabled = false;
        enabled = false;
    }

    private void Flip(){
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        m_direction = transform.localScale.x;
    }

    public void OnDetected(Collider2D collider){
        ChaseEnemy(collider.transform);
    }

    public void OnLost(Collider2D collider){
        if(state != State.IDLE)
            CheckEnemies();
    }

    private void OnDrawGizmosSelected(){
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }


    enum State{
        IDLE,
        CHASE,
        COMBAT
    }
}
