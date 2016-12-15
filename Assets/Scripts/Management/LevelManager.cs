using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    public static LevelManager instance = null;     //Allows other scripts to call functions from SoundManager.             


    void Awake()
    {
        //Check if there is already an instance of SoundManager
        if (instance == null)
            //if not, set it to this.
            instance = this;
        //If instance already exists:
        else if (instance != this)
            //Destroy this, this enforces our singleton pattern so there can only be one instance of SoundManager.
            Destroy(gameObject);

        //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
        DontDestroyOnLoad(gameObject);
    }

    public enum layer_masks
    {
        platform = 8,
        item = 9,
        player = 10,
        enemy = 11,
        held_item = 12
    }

    void Start()
    {
        Physics2D.IgnoreLayerCollision((int)layer_masks.player, (int)layer_masks.enemy, true);
        Physics2D.IgnoreLayerCollision((int)layer_masks.player, (int)layer_masks.item, true);
        Physics2D.IgnoreLayerCollision((int)layer_masks.held_item, (int)layer_masks.platform, true);
        Physics2D.IgnoreLayerCollision((int)layer_masks.held_item, (int)layer_masks.enemy, true);
    }
}
