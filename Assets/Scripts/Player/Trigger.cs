using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    private void Update()
    {
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("collision enter 2d");
    }
    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D other)
    {
        var controller = transform.parent.GetComponent<Controller>();

        // If the player enters the trigger zone...
        if (other.gameObject.tag == "Shell")
        {
            var shell = other.gameObject.GetComponent<Shell>();

            if (shell.is_moving())
            {
                // ray tracing
                var ray = controller.transform.FindChild("ray");

                var ray_start = ray.transform.position;
                var ray_top = new Vector2(ray.transform.position.x, ray.transform.position.y - 8);

                RaycastHit2D[] top_hits = Physics2D.LinecastAll(ray_start, ray_top);

                foreach (RaycastHit2D hit in top_hits)
                {
                    var collider = hit.collider;
                    if (collider != null)
                    {
                        if (collider.gameObject.tag == "Shell")
                        {
                            var player_rigid_body = controller.GetComponent<Rigidbody2D>();
                            player_rigid_body.velocity = new Vector2(player_rigid_body.velocity.x, 0);

                            if (Input.GetKey(KeyCode.Space))
                            {
                                player_rigid_body.AddForce(new Vector2(0, controller.SmallJumpForce * 2f));
                            }
                            else
                            {
                                player_rigid_body.AddForce(new Vector2(0, controller.SmallJumpForce * 1.25f));
                            }
                            
                            SoundManager.instance.PlaySingle(controller.StompSoundEffect);

                            shell.is_moving(false);
                            var rigid_body = shell.GetComponent<Rigidbody2D>();
                            rigid_body.velocity = new Vector2();

                        }
                    }
                }

            }
            else
            {
                // verify pressed key
                if (Input.GetKey(KeyCode.Z))
                {
                    controller.pick_item_up(other.gameObject);
                }
                else
                {
                    controller.kick_item(other.gameObject);
                }
            }
        } else if(other.gameObject.tag == "Mushroom")
        {
            Destroy(other.gameObject);
            controller.trigger_power_change(Controller.power_status.mushroom);
        }

    }
}
