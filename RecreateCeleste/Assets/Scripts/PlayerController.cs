using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    public LayerMask groundLayer; // 地面层
    public LayerMask wallLayer;

    public float moveSpeed = 5f; // 移动速度
    public float jumpForce = 10f; // 跳跃力度
    public float slideSpeed = 4f;
    public float wallClimbSpeed = 2f;
    public float wallJumpHoriSpeed = 2.5f;
    
    
    public float wallDetectRange = 0.2f;
    public float wallDetectHoriOffset = 0.2f;
    
    public float fallGravityMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    
    public float jumpUndetectTime = 0.5f;
    
    public bool isJumpable;
    public bool isOnLeftWall;
    public bool isJumping;
    public bool isOnWall;
    public bool isGrounded; // 是否在地面上
    public bool isPassStatusCheck;
    
    private Rigidbody2D rb;
    private Collider2D coll;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // 获取 Rigidbody2D 组件
        coll = GetComponent<Collider2D>(); // 获取 Collider2D 组件
    }

    void Update()
    {
        StatusDetect();
        Move(); // 控制左右移动
        Jump(); // 控制跳跃
        Wallslide();
        FineTune();
    }

    private void StatusDetect()
    {
        if (isPassStatusCheck)
        {
            return;
        }

        
        isOnLeftWall = Physics2D.OverlapCircle((Vector2)transform.position - Vector2.right * wallDetectHoriOffset, wallDetectRange, wallLayer);
        isOnWall = Physics2D.OverlapCircle((Vector2)transform.position + Vector2.right * wallDetectHoriOffset, wallDetectRange, wallLayer)
            || Physics2D.OverlapCircle((Vector2)transform.position - Vector2.right * wallDetectHoriOffset, wallDetectRange, wallLayer);
        // 使用射线检测是否与地面层接触，更新 isGrounded
        isGrounded = Physics2D.IsTouchingLayers(coll, groundLayer);
        
        if ((isGrounded || isOnWall) && !isJumpable)
        {
            isJumpable = true;
        }

        if (isGrounded || isOnWall)
        {
            isJumping = false;
        }
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
        if (Input.GetKeyDown(KeyCode.Space) && isJumpable)
        {
            Debug.Log("Jump");
            
            if (isOnWall && isOnLeftWall)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x + wallJumpHoriSpeed, jumpForce);
            }
            else if (isOnWall && !isOnLeftWall)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x - wallJumpHoriSpeed, jumpForce);
            }
            else
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
            Debug.Log(rb.linearVelocity);

            
            isGrounded = false; // 跳跃后设置为不在地面上
            isJumpable = false;
            isJumping = true;
            isOnWall = false;
            StartCoroutine(PassStatusCoroutine(jumpUndetectTime));
        }
    }

    private void Wallslide()
    {
        if (isJumping)
        {
            return;
        }
        float yInput = Input.GetAxis("Vertical");
        
        if (!isGrounded && isOnWall && !Input.GetKey(KeyCode.LeftShift))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -slideSpeed);
        }
        else if (!isGrounded && isOnWall && Input.GetKey(KeyCode.LeftShift))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, yInput * wallClimbSpeed);
        }

    }
    
    private IEnumerator PassStatusCoroutine(float waitTime)
    {
        // 将状态设置为 false
        isPassStatusCheck = true;

        // 等待指定的时间
        yield return new WaitForSeconds(waitTime);

        // 将状态设置为 true
        isPassStatusCheck = false;
    }


}