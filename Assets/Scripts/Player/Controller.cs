using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {

    public float WalkSpeed = 768f;
    public float KickPower = 8192f;
    public float JumpForce = 14000f;
    public float TopWalkSpeed = 128f;
    public float TopRunSpeed = 128f;
    public float TopSpeed = 128f;
    float TopSpeedBound = 128f;

    public LayerMask PlatformMask;

    Animator animator;

    GameObject item_held;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        TopSpeedBound = TopSpeed;
    }

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void pick_item_up(GameObject item)
    {
        if (item_held == null)
        {
            item_held = item;
            item_held.transform.parent = this.transform;

            var item_rigid_body = item.GetComponent<Rigidbody2D>();
            Destroy(item_rigid_body);
        }
    }

    public void pick_item_down()
    {
        if (item_held != null)
        {
            item_held.transform.parent = null;
            item_held.gameObject.AddComponent<Rigidbody2D>();

            var item_rigid_body = item_held.GetComponent<Rigidbody2D>();
            item_rigid_body.freezeRotation = true;

            kick_item(item_held);

            item_held = null;
        }

    }
    public void kick_item(GameObject item)
    {
        var item_rigid_body = item.GetComponent<Rigidbody2D>();
        var rigid_body = GetComponent<Rigidbody2D>();
        var local_scale = rigid_body.transform.localScale;

        item_rigid_body.AddForce(new Vector2(KickPower * local_scale.x, 0));

        if(item.GetComponent<AudioSource>() != null)
        {
            item.GetComponent<AudioSource>().Play();
        }
    }

    void FixedUpdate()
    {


        float x_input = Input.GetAxis("Horizontal");
        float y_input = Input.GetAxis("Vertical");

        float x_movement = 0.0f;

        x_movement = x_input * WalkSpeed;
        // crouching happens when y_input is less than 0 (down axis pressed)
        bool is_crouching = y_input < 0;

        // ground collider
        CircleCollider2D collider = GetComponent<CircleCollider2D>();

        bool on_ground = Physics2D.OverlapCircle(collider.transform.position,
                                                 collider.radius,
                                                 PlatformMask);
        // x_input is being left pressed
        if (x_input < 0)
        {
            if (!is_crouching || (is_crouching && !on_ground)) 
                // can only change if not crouching or crouching and jumping
            {
                transform.localScale = new Vector3(-1, 1, 1); // swapping local scale (sprite facing left)
            }
            
        }
        else if (x_input > 0) // x_input is being right pressed
        {
            if (!is_crouching || (is_crouching && !on_ground))
            // can only change if not crouching or crouching and jumping
            {
                transform.localScale = new Vector3(1, 1, 1);  // swapping local scale (sprite facing right)
            }
            
        }

        // x_input != 0 means horizontal movement
        if (x_input != 0)
        {
            animator.SetFloat("x_speed", 1);
        }
        else // x_input == 0 means no horizontal movement
        {
            animator.SetFloat("x_speed", -1);
        }

     
        if (is_crouching)
        {
            animator.SetBool("is_crouching", true); // setting animator parameter
        }
        else
        {
            animator.SetBool("is_crouching", false); // setting is_crouching animator parameter
        }

        bool is_looking_up = y_input > 0 && Mathf.Abs(x_input) < 0.1f; // can't look up while walking
        if (is_looking_up)
        {
            animator.SetBool("is_looking_up", true); // setting animator parameter
        }
        else
        {
            animator.SetBool("is_looking_up", false); // setting is_looking_up animator parameter
        }

        // crouching
        if ((!is_crouching && !is_looking_up) || (is_crouching && !on_ground)) 
            // only move when not crouching and not looking up or drifting
        {

            var rigid_body = GetComponent<Rigidbody2D>();

            rigid_body.AddForce(Vector2.right * x_movement);
            /*transform.position = new Vector3(transform.position.x + x_movement, 
                                             transform.position.y, 
                                             transform.position.z);*/

            Vector3 clamp_velocity= rigid_body.velocity;
            clamp_velocity.x = Mathf.Clamp(clamp_velocity.x, -TopSpeed, TopSpeed);
            rigid_body.velocity = clamp_velocity;

            animator.SetFloat("y_speed", clamp_velocity.y);


            // drifting

            if(clamp_velocity.x > 0 && x_input < 0.0)
            {
                animator.SetBool("is_drifting", true);
                animator.SetInteger("drift", 1);
            } else if(clamp_velocity.x < 0 && x_input > 0)
            {
                animator.SetBool("is_drifting", true);
                animator.SetInteger("drift", 1);
            } else
            {
                animator.SetBool("is_drifting", false);
                animator.SetInteger("drift", 0);
            }
        }
      
        // setting animation parameter on_ground
        animator.SetBool("on_ground", on_ground);

        // validation for jumping
        if (Input.GetKeyDown(KeyCode.Space) && on_ground)   {
            GetComponent<Rigidbody2D>().AddForce(new Vector2(0, JumpForce)); // adding jump force
        }

        // validation for running
        if (Input.GetKey(KeyCode.Z) && x_movement != 0) 
        {
            is_running = true;

            animator.SetBool("is_running", is_running);

            TopSpeed = TopSpeedBound + TopSpeedBound * 0.25f;

            if (is_turbo)
            {
                TopSpeed = TopSpeedBound + TopSpeedBound * 0.65f;
            }
            else
            {
                StartCoroutine("verify_turbo");
            }

        }
        else
        {
            TopSpeed = TopSpeedBound;
            is_running = false;
            is_turbo = false;

            animator.SetBool("is_running", is_running);
            animator.SetBool("is_turbo", is_turbo);

            if(item_held != null && !Input.GetKey(KeyCode.Z))
            {
                pick_item_down();
            }

        }
    }
   
   

    private IEnumerator verify_turbo()
    {
        yield return new WaitForSeconds(2);

        if (is_running)
        {
            is_turbo = true;
            animator.SetBool("is_turbo", is_turbo);
        }
        else
        {
            StopCoroutine("verify_turbo");
        }

    }

    bool is_running = false;
    bool is_turbo = false;

    void OnCollisionEnter2D(Collision2D other)
    {
        // If the player enters the trigger zone...
        if (other.gameObject.tag == "CarryItem")
        {
            var shell = other.gameObject.GetComponent<Shell>();

            if (shell.is_moving)
            {
                shell.is_moving = false;
                var rigid_body = shell.GetComponent<Rigidbody2D>();
                rigid_body.velocity = new Vector2();
            }
            else
            {
                // verify pressed key
                if (Input.GetKey(KeyCode.Z))
                {
                    pick_item_up(other.gameObject);
                }
                else
                {
                    kick_item(other.gameObject);
                }
            }                        
        }

    }
    /*private void OnCollisionEnter(Collision collision) //@TODO
    {
        //collision.gameObject.layer == PlatformMask && collision.transform.nor
    }*/


}
