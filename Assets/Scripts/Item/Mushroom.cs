using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : MonoBehaviour {

    public bool MoveFromStart = false;

    bool _moving = false;

    float _speed_x = 128f;

    Rigidbody2D _rigid_body;

	// Use this for initialization
	void Start () {
        if (MoveFromStart)
        {
            this.enable_movement();
        }

        this._rigid_body = this.GetComponent<Rigidbody2D>();

    }

    public void enable_movement()
    {
        this._moving = true;
    }

    public void spawn()
    {
        int direction = (int) Random.Range(1, 2);

        switch (direction)
        {
            case 2:
                this._speed_x *= -1;
            break;
        }

        this._rigid_body.AddForce(new Vector2(this._speed_x, 768f));
    }

	// Update is called once per frame
	void FixedUpdate () {

        if (this._moving)
        {
            this._rigid_body.velocity = new Vector3(this._speed_x, 0);
        }
    }
}
