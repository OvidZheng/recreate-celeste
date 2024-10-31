using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // 移动速度
    public float jumpForce = 10f; // 跳跃力度
    public LayerMask groundLayer; // 地面层
    
    public float fallGravityMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    public bool isOnWall;
    private bool isGrounded; // 是否在地面上
    
    private Rigidbody2D rb;
    private Collider2D coll;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // 获取 Rigidbody2D 组件
        coll = GetComponent<Collider2D>(); // 获取 Collider2D 组件
    }

    void Update()
    {
        Move(); // 控制左右移动
        Jump(); // 控制跳跃
        FineTune();
    }

    private void FineTune()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallGravityMultiplier - 1) * Time.deltaTime;
        }
        else if(rb.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    void Move()
    {
        float moveInput = Input.GetAxis("Horizontal"); // 获取左右移动输入
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y); // 设置左右移动速度
    }

    void Jump()
    {
        // 如果按下空格键且角色在地面上
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce); // 设置跳跃速度
            isGrounded = false; // 跳跃后设置为不在地面上
        }
    }

    private void FixedUpdate()
    {
        // 使用射线检测是否与地面层接触，更新 isGrounded
        isGrounded = Physics2D.IsTouchingLayers(coll, groundLayer);
    }
}