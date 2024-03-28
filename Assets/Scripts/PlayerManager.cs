using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] LayerMask blockLayer;
    public enum DIRECTION_TYPE
    {
        STOP,
        RIGFT,
        LEFT,
    }

    DIRECTION_TYPE direction = DIRECTION_TYPE.STOP;

    new Rigidbody2D rigidbody2D;
    float speed;

    private Animator animator;

    [SerializeField] AudioClip getItemSE;

    [SerializeField] AudioClip jumpSE;

    [SerializeField] AudioClip stampSE;

    AudioSource audioSource;

    float jumpPower = 400;
    private void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        float x = Input.GetAxis("Horizontal");
        animator.SetFloat("speed", Mathf.Abs(x));

        if (x == 0)
        {
            direction = DIRECTION_TYPE.STOP;
        }
        else if (x > 0)
        {
            direction = DIRECTION_TYPE.RIGFT;
        }
        else if(x < 0)
        {
            direction = DIRECTION_TYPE.LEFT;
        }

        if (IsGround() && Input.GetKeyDown("space"))
        {
            jump();
        }
    }

    private void FixedUpdate()
    {
        switch (direction)
        {
            case DIRECTION_TYPE.STOP:
                speed = 0;
                break;
            case DIRECTION_TYPE.RIGFT:
                speed = 3;
                transform.localScale = new Vector3(1,1,1);
                break;
            case DIRECTION_TYPE.LEFT:
                speed = -3;
                transform.localScale = new Vector3(-1, 1, 1);
                break;
        }
        rigidbody2D.velocity = new Vector2(speed, rigidbody2D.velocity.y);
    }

    void jump()
    {
        rigidbody2D.AddForce(Vector2.up * jumpPower);
        //Debug.Log("jump");
        audioSource.PlayOneShot(jumpSE);
    }

    bool IsGround()
    {
        Vector3 leftStartPoint = transform.position - Vector3.right * 0.2f;
        Vector3 rigftStartPoint = transform.position + Vector3.right * 0.2f;
        Vector3 endPoint = transform.position - Vector3.up * 0.1f;
        Debug.DrawLine(leftStartPoint, endPoint);
        Debug.DrawLine(rigftStartPoint, endPoint);
        return Physics2D.Linecast(leftStartPoint,endPoint,blockLayer)
            || Physics2D.Linecast(rigftStartPoint, endPoint, blockLayer);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Trap")
        {
            Debug.Log("ゲームオーバー");
            gameManager.GameOver();
        }
        if (collision.gameObject.tag == "Finish")
        {
            Debug.Log("クリア");
            gameManager.GameClear();
        }
        if (collision.gameObject.tag == "Item")
        {
            audioSource.PlayOneShot(getItemSE);
            collision.gameObject.GetComponent<ItemManager>().GetItem();
        }

        if (collision.gameObject.tag == "Enemy")
        {
            EnemyManager enemy = collision.gameObject.GetComponent<EnemyManager>();
            if (this.transform.position.y + 0.2f > enemy.transform.position.y)
            {
                rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0);
                jump();
                audioSource.PlayOneShot(stampSE);
                enemy.DestroyEnemy();
            }
            else
            {
                Destroy(this.gameObject);   
                gameManager.GameOver(); 
            }
        }


    }

    

}
