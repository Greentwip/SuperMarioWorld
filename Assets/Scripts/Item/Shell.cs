using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour {

    float XSpeed;
    float Acceleration = 96;
    float TopSpeed = 280;
    float ray_length = 16;
    float ray_offset = 8;

    bool _is_moving = false;


    Animator animator;

    public AudioClip RicochetSound;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Use this for initialization
    public bool is_moving()
    {
        return this._is_moving;
    }

    public void is_moving(bool moving)
    {
        this._is_moving = moving;
    }

	// Use this for initialization
	void Start () {
    }


    void FixedUpdate () {

        var rigid_body = GetComponent<Rigidbody2D>();

        if(rigid_body != null) // means it's not currently being held by the player
        { 
            XSpeed = rigid_body.velocity.x;

            // moving to left
            if (XSpeed < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1); // swapping local scale (sprite facing left)
                rigid_body.AddForce(new Vector2(-Acceleration, 0));
            }
            else if (XSpeed > 0) // moving to right
            {
                transform.localScale = new Vector3(1, 1, 1);  // swapping local scale (sprite facing right)
                rigid_body.AddForce(new Vector2(Acceleration, 0));

            }

            if (XSpeed != 0)
            {
                _is_moving = true;
            }
            else
            {
                _is_moving = false;
            }


            animator.SetBool("is_moving", _is_moving);

            Vector3 clamp_velocity = rigid_body.velocity;
            clamp_velocity.x = Mathf.Clamp(clamp_velocity.x, -TopSpeed, TopSpeed);
            rigid_body.velocity = clamp_velocity;

            // ray tracing
            var ray = transform.FindChild("ray");

            var ray_start = new Vector2(ray.transform.position.x, ray.transform.position.y + ray_offset);
            var ray_right = new Vector2(ray.transform.position.x + ray_length, ray.transform.position.y + ray_offset);
            var ray_left = new Vector2(ray.transform.position.x - ray_length, ray.transform.position.y + ray_offset);

            RaycastHit2D[] right_hits = Physics2D.LinecastAll(ray_start, ray_right);
            RaycastHit2D[] left_hits  = Physics2D.LinecastAll(ray_start, ray_left);

            Debug.DrawLine(ray_start, ray_right, Color.red);
            Debug.DrawLine(ray_start, ray_left, Color.red);

            foreach (RaycastHit2D hit in right_hits)
            {
                var collider = hit.collider;
                if (collider != null)
                {
                    if (collider.gameObject.tag == "Platform")
                    {
                        rigid_body.velocity = new Vector2(-Mathf.Abs(rigid_body.velocity.x), rigid_body.velocity.y);
                        rigid_body.AddForce(new Vector2(-TopSpeed, 0));
                        SoundManager.instance.PlaySingle(RicochetSound); 
                    }
                }
            }

            foreach (RaycastHit2D hit in left_hits)
            {
                var collider = hit.collider;
                if (collider != null)
                {
                    if (collider.gameObject.tag == "Platform")
                    {
                        rigid_body.velocity = new Vector2(Mathf.Abs(rigid_body.velocity.x), rigid_body.velocity.y);
                        rigid_body.AddForce(new Vector2(TopSpeed, 0));
                        SoundManager.instance.PlaySingle(RicochetSound);
                    }
                }
            }
         

        }

    }
}
