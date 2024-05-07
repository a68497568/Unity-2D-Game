using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySmart : MonoBehaviour
{
    private Rigidbody2D rig;//鋼體
    private Animator Anim;//動畫器
    [Header("Layers")]
    public LayerMask playerLayer;//用來開啟layer
    [Space]
    [Header("Collision")]
    private Collider2D coll;//碰撞器
    [SerializeField] private float collisionRadius = 5f;//檢測碰撞半徑
    Vector2 beg;//射線起點
    Vector2 down = new Vector2(0, -1);//控制射線角度的向量
    [SerializeField] private float radialLength = 1.1f;//射線的長度
    [Space]
    [Header("Speed")]
    private float moveSpeed = 200f;//移動速度
    [SerializeField] private float face;//朝向
    public float attackRange = 2f;
    Transform player;
    private bool isAttack = false;
    public int DeathCode = 0;

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();//獲取鋼體組件
        Anim = GetComponent<Animator>();//獲取動畫組件
        coll = GetComponent<CircleCollider2D>();//獲取碰撞器
        face = -1;//初始朝向是負的，和角色不同的地方，face-1代表朝向左邊
        playerLayer = 1 << 9;//把玩家Layer放在Layer9
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }


    void FixedUpdate()
    {
        ChangeAnimator();
        beg = transform.position;
        Collider2D playerColl = isPlayerView();
        if (isBorder())//是否到邊緣
        {
            
            rig.velocity = new Vector2(0, 0);
            AccordingDirectionFlip(playerColl);
            Anim.SetInteger("AnimState", 0);
        }
        else if(!isBorder() && DeathCode == 0) //不到邊緣就可以移動
        {
            AccordingDirectionFlip(playerColl);
            Move();
        }
    }

    void AccordingDirectionFlip(Collider2D playerColl)//根據玩家是否出現在視野中，安排敵人轉向
    {
        if (playerColl != null)//如果玩家出現在視野中
        {
            int direction;
            if (playerColl.transform.position.x < transform.position.x)
            {
                direction = -1;//玩家在敵人的左邊
            }
            else
            {
                direction = 1;//玩家在敵人的右邊
            }
            if (direction != face)//表示方向不一致
            {
                //Debug.Log(direction);
                Flip();
            }
        }
    }
    void Flip()//翻轉角色方向
    {
        face = (face == 1) ? -1 : 1;
        transform.localScale = new Vector2(face * (-1), 1);//乘-1是因為初始動畫朝向是朝著左邊的，但是初始座標卻是1，是相反的
    }

    bool isBorder()//判斷是否已抵達邊界
    {
        //也可以使用Debug的方式可視化射線
        //Debug.DrawLine(beg, beg + (new Vector2(face, 0) + down) * radialLength, Color.red);
        if (!Physics2D.Raycast(beg, new Vector2(face, 0) + down, radialLength, LayerMask.GetMask("Ground")))//抵達邊界
        {
            return true;
        }
        return false;
    }

    Collider2D isPlayerView()//判斷玩家是否進入視野
    {
        return Physics2D.OverlapCircle((Vector2)transform.position, collisionRadius, playerLayer);//判斷是否碰到地面
    }

    void Move()//左右移動
    {
        rig.velocity = new Vector2(face * moveSpeed * Time.deltaTime, rig.velocity.y);//輸入x，y向量，數值*方向      
        if (Vector2.Distance(player.position, rig.position) <= attackRange && !isAttack)
        {
            Anim.SetTrigger("Attack");
            isAttack = true;
        }
    }

    void Attacking()
    {
        isAttack = false;
    }

    void ChangeAnimator()//動畫狀態轉換
    {
        Anim.SetFloat("speed", Mathf.Abs(rig.velocity.x));//速度是向量
        if (Mathf.Abs(face) > Mathf.Epsilon)
            Anim.SetInteger("AnimState", 2);
    }

    void Destroy()
    {
        Destroy(gameObject);
    }

    public void DeadState(int DTCode)
    {
        DeathCode = DTCode;
    }

    void OnDrawGizmos()//繪製輔助線
    {
        Gizmos.color = Color.red;//輔助線顏色
        Gizmos.DrawWireSphere((Vector2)transform.position, collisionRadius);//繪製射線
        Gizmos.DrawLine((Vector2)transform.position, (Vector2)transform.position + (new Vector2(face, 0) + down) * radialLength);//繪製圓形
    }
}
