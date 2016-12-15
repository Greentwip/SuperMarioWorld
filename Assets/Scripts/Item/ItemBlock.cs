using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBlock : MonoBehaviour {
    float ray_length = 16;
    float ray_offset = 8;

    bool is_active = true;

    Animator animator;

    public enum ItemKind
    {
        Mushroom,
        Coin,
        Feather,
        Flower
    }

    public ItemKind Itemkind;

    

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        var rigid_body = GetComponent<Rigidbody2D>();

        // ray tracing
        var ray = transform.FindChild("ray");

        var ray_start = new Vector2(ray.transform.position.x, ray.transform.position.y + ray_offset);
        var ray_left = new Vector2(ray.transform.position.x - ray_length, ray.transform.position.y + ray_offset);
        var ray_right = new Vector2(ray.transform.position.x + ray_length, ray.transform.position.y + ray_offset);

        RaycastHit2D[] left_hits = Physics2D.LinecastAll(ray_start, ray_left);
        RaycastHit2D[] right_hits = Physics2D.LinecastAll(ray_start, ray_right);

        Debug.DrawLine(ray_start, ray_left, Color.red);
        Debug.DrawLine(ray_start, ray_right, Color.red);

        foreach (RaycastHit2D hit in left_hits)
        {
            var collider = hit.collider;
            if (collider != null)
            {
                if (collider.gameObject.tag == "Shell")
                {
                    var shell = collider.gameObject.GetComponent<Shell>();
                    if (shell.is_moving())
                    {
                        is_active = false;
                    }
                }
            }
        }

        foreach (RaycastHit2D hit in right_hits)
        {
            var collider = hit.collider;
            if (collider != null)
            {
                if (collider.gameObject.tag == "Shell")
                {
                    var shell = collider.gameObject.GetComponent<Shell>();
                    if (shell.is_moving())
                    {
                        is_active = false;
                    }
                    
                }
            }
        }

        animator.SetBool("is_active", is_active);

    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Shell")
        {
            var shell = other.gameObject.GetComponent<Shell>();

            if (shell.is_moving())
            {
                
            }
        }
        else
        {
        }
    }
}
