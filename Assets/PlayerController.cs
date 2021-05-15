using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 4.0f;
    public float jumpForce = 7.5f;

    public Transform attackPoint;
    public LayerMask enemyLayers;
    public float attackRange = 5f;
    public float maxHealth = 100f;
    public float damage = 40f;
    public float attackDelay = 0.5f;
    public float wallJumpForce = 15f;

    private float curAttackDelay = 0f;
    private Animator m_animator;
    private Rigidbody2D m_body2d;
    private Sensor_Bandit m_groundSensor;
    private float curHealth;
    private Sensor_Bandit m_wallSensor;
    private bool m_wallJump;

    
    void Start(){
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_Bandit>();
        curHealth = maxHealth;
        m_wallSensor = transform.Find("WallSensor").GetComponent<Sensor_Bandit>();
        m_animator.SetInteger("AnimState", 1);
    }

    
    void Update(){
        var input = Input.GetAxis("Horizontal");

        if(input > 0)
            transform.localScale = new Vector2(-1, 1);
        else if(input < 0)
            transform.localScale = new Vector2(1, 1);

        if(Input.GetButtonDown("Jump"))
            Jump();
        if(m_wallJump)
            m_body2d.velocity = new Vector2(transform.localScale.x * speed * -1, jumpForce);
        else
            Move(input * speed);
        UpdateAnimator(input);
    }

    private void Jump(){
        if(m_groundSensor.State())
            m_body2d.velocity = new Vector2(m_body2d.velocity.x, jumpForce);
        else if(m_wallSensor.State()){
            transform.localScale = new Vector2(transform.localScale.x * -1, 1);
            m_wallJump = true;
            Invoke("SetWallJumpToFalse", 0.08f);
        }
    }

    private void SetWallJumpToFalse(){
        m_wallJump = false;
    }

    private void UpdateAnimator(float input){
        if(input != 0)
            m_animator.SetInteger("AnimState", 2);
        else
            m_animator.SetInteger("AnimState", 1);
    }

    private void Move(float move){
        m_body2d.velocity = new Vector2(move, m_body2d.velocity.y);
    }

    private void Attack(){
        
    }

    public void Hurt(float damage){
        
    }

    private void OnDrawGizmosSelected(){
        
    }

    private void Die(){
        
    }
}
