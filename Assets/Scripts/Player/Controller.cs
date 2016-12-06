using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {

    public float WalkSpeed = 768f;
    public float JumpForce = 14000f;
    public float TopWalkSpeed = 128f;
    public float TopRunSpeed = 128f;
    public float TopSpeed = 128f;

    public LayerMask PlatformMask;

    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();

    }

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}


    bool is_drifting = false; 

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
            animator.SetFloat("x_speed", 0);
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

        }
      
        // setting animation parameter on_ground
        animator.SetBool("on_ground", on_ground);

        // validation for jumping
        if (Input.GetKeyDown(KeyCode.Space) && on_ground){
            GetComponent<Rigidbody2D>().AddForce(new Vector2(0, JumpForce)); // adding jump force
        }
    }

    /*private void OnCollisionEnter(Collision collision) //@TODO
    {
        //collision.gameObject.layer == PlatformMask && collision.transform.nor
    }*/


}
