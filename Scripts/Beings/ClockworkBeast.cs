using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ClockworkBeast : Enemy {

    public Enemy owner;
    public Vector3 originalLoc = Vector3.zero;
    public int shipPos = -1;    //if -1, not tracking

    public bool debug_kill = false;

	//Clockwork Beast sprite layer values
	float LEGS_ORDER = 0.0f;
	float LEFT_ARM_ORDER = -0.1f;
	float LEFT_ARM_CLOCK_ORDER = -0.2f;
	float TORSO_ORDER = -0.2f;
	float TORSO_CLOCK_ORDER = -0.3f;
	float RIGHT_ARM_ORDER = -0.6f;
	float RIGHT_ARM_CLOCK_ORDER = -0.7f;
	float RIGHT_ARM_MODIFICATION_ORDER = -0.7f;
	float HEAD_ORDER = -0.4f;
	float HEAD_CLOCK_ORDER = -0.5f;
	float MOUTH_ORDER = -0.5f;

	int LEGS_INDEX = 0;
	int LEFT_ARM_INDEX = 1;
	int LEFT_ARM_CLOCK_INDEX = 2;
	int TORSO_INDEX = 3;
	int TORSO_CLOCK_INDEX = 4;
	int RIGHT_ARM_INDEX = 5;
	int RIGHT_ARM_CLOCK_INDEX = 6;
	int RIGHT_ARM_MODIFICATION_INDEX = 7;
	int HEAD_INDEX = 8;
	int HEAD_CLOCK_INDEX = 9;
	int MOUTH_INDEX = 10;

    int animation_number = 4;

    public List<Sprite> spriteLegs;
    public List<Sprite> spriteLeftArm;
    public List<Sprite> spriteLeftArmClocks;
    public List<Sprite> spriteTorso;
    public List<Sprite> spriteTorsoClocks;
    public List<Sprite> spriteRightArm;
    public List<Sprite> spriteRightArmClocks;
    public List<Sprite> spriteRightArmMod;
    public List<Sprite> spriteHead;
    public List<Sprite> spriteHeadClocks;
    public List<Sprite> spriteMouth;

	//initialize
	public void Awake(){

        BaseInit();

		//create Clockwork Beast form
        animator = (Sprite_Animator)Instantiate(Resources.Load("Prefabs\\Animation\\Sprite_Animator", typeof(Sprite_Animator)));
        animator.transform.parent = transform;
        animator.gameObject.layer = 10;
        animator.transform.localPosition = Vector3.zero;

        animator.transform.gameObject.name = "Body";

        //Legs
        animator.AddLayer (spriteLegs, ColorHelper.GetColorRandom ());
        animator.SetLayerOrder (LEGS_INDEX, LEGS_ORDER);
        animator.SetIterator (0, 0, 0 + animation_number-1, LEGS_INDEX);
        animator.SetLayerTitle (LEGS_INDEX, "Legs");
        animator.sprite_layers[0].gameObject.layer = 10;

        //Left Arm
        animator.AddLayer ( spriteLeftArm, ColorHelper.GetColorRandom ());
        animator.SetLayerOrder (LEFT_ARM_INDEX,LEFT_ARM_ORDER);
        animator.SetIterator (0, 0, 0 + animation_number-1, LEFT_ARM_INDEX);
        animator.SetLayerTitle (LEFT_ARM_INDEX, "Left Arm");
        animator.sprite_layers[1].gameObject.layer = 10;

        //Left Arm Clock
        animator.AddLayer ( spriteLeftArmClocks, ColorHelper.GetColorRandom ());
        animator.SetLayerOrder (LEFT_ARM_CLOCK_INDEX,LEFT_ARM_CLOCK_ORDER);
        animator.SetIterator (0, 0, 0 + animation_number-1, LEFT_ARM_CLOCK_INDEX);
        animator.SetLayerTitle (LEFT_ARM_CLOCK_INDEX, "Left Arm Clock");
        animator.sprite_layers[2].gameObject.layer = 10;

        //Torso
        animator.AddLayer ( spriteTorso, ColorHelper.GetColorRandom ());
        animator.SetLayerOrder (TORSO_INDEX,TORSO_ORDER);
        animator.SetIterator (0, 0, 0 + animation_number-1, TORSO_INDEX);
        animator.SetLayerTitle (TORSO_INDEX, "Torso");
        animator.sprite_layers[3].gameObject.layer = 10;

        //Torso Clock(s)
        animator.AddLayer ( spriteTorsoClocks, ColorHelper.GetColorRandom ());
        animator.SetLayerOrder (TORSO_CLOCK_INDEX,TORSO_CLOCK_ORDER);
        animator.SetIterator (0, 0, 0 + animation_number-1, TORSO_CLOCK_INDEX);
        animator.SetLayerTitle (TORSO_CLOCK_INDEX, "Torso Clocks");
        animator.sprite_layers[4].gameObject.layer = 10;

        //Right Arm
        animator.AddLayer ( spriteRightArm, ColorHelper.GetColorRandom ());
        animator.SetLayerOrder (RIGHT_ARM_INDEX,RIGHT_ARM_ORDER);
        animator.SetIterator (0, 0, 0 + animation_number-1, RIGHT_ARM_INDEX);
        animator.SetLayerTitle (RIGHT_ARM_INDEX, "Right Arm");
        animator.sprite_layers[5].gameObject.layer = 10;

        //Right Arm Clock
        animator.AddLayer ( spriteRightArmClocks, ColorHelper.GetColorRandom ());
        animator.SetLayerOrder (RIGHT_ARM_CLOCK_INDEX,RIGHT_ARM_CLOCK_ORDER);
        animator.SetIterator (0, 0, 0 + animation_number-1, RIGHT_ARM_CLOCK_INDEX);
        animator.SetLayerTitle (RIGHT_ARM_CLOCK_INDEX, "Right Arm Clock");
        animator.sprite_layers[6].gameObject.layer = 10;

        //Right Arm Modification
        animator.AddLayer ( spriteRightArmMod, ColorHelper.GetColorRandom ());
        animator.SetLayerOrder (RIGHT_ARM_MODIFICATION_INDEX,RIGHT_ARM_MODIFICATION_ORDER);
        animator.SetIterator (0, 0, 0 + animation_number-1, RIGHT_ARM_MODIFICATION_INDEX);
        animator.SetLayerTitle (RIGHT_ARM_MODIFICATION_INDEX, "Right Arm Modification");
        animator.sprite_layers[7].gameObject.layer = 10;

        //Head
        animator.AddLayer ( spriteHead, ColorHelper.GetColorRandom ());
        animator.SetLayerOrder (HEAD_INDEX,HEAD_ORDER);
        animator.SetIterator (0, 0, 0 + animation_number-1, HEAD_INDEX);
        animator.SetLayerTitle (HEAD_INDEX, "Head");
        animator.sprite_layers[8].gameObject.layer = 10;

        //Head Clocks
        animator.AddLayer ( spriteHeadClocks, ColorHelper.GetColorRandom ());
        animator.SetLayerOrder (HEAD_CLOCK_INDEX,HEAD_CLOCK_ORDER);
        animator.SetIterator (0, 0, 0 + animation_number-1, HEAD_CLOCK_INDEX);
        animator.SetLayerTitle (HEAD_CLOCK_INDEX, "Head Clock");
        animator.sprite_layers[9].gameObject.layer = 10;

        //Mouth
        animator.AddLayer ( spriteMouth, ColorHelper.GetColorRandom ());
        animator.SetLayerOrder (MOUTH_INDEX,MOUTH_ORDER);
        animator.SetIterator (0, 0, 0 + animation_number-1, MOUTH_INDEX);
        animator.SetLayerTitle (MOUTH_INDEX, "Mouth");
        animator.sprite_layers[10].gameObject.layer = 10;

        animator.UpdateSprite ();
        animator.transform.localScale = new Vector3(1, 1, 1);
        animator.transform.localPosition = new Vector3(0, 0, 1);
		
		
	}//eof Awake

    void Start()
    {
        tag = ENEMY;
        OnRecieveDamage = HandleRBK;
        loseOnRecieveDamageMethodOnActivation = false;
    }

    public void SetHealth(int h)
    {
        currentHealth = maxHealth = h;
    }

    bool HandleRBK()
    {
        if (currentHealth == 1)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
        }
        return true;
    }

    protected override void SetSpeedMod()
    {
        speedMod = 0;
    }

    protected override IEnumerator OnEndOfDeathProcess()
    {
        transform.parent = null;
        if (owner != null)
            owner.RecieveDamage(1, Vector3.up);
        yield return null;
        //gameObject.SetActive(false);
    }

    public override IEnumerator Move()
    {
        animator.enabled = true;
        if (rb != null)
        {
            Destroy(rb);
            rb = null;
            transform.parent = owner.transform;
            transform.localPosition = originalLoc;
        }
        GetComponent<Collider2D>().enabled = true;
        if(originalLoc==Vector3.zero)
            originalLoc = transform.localPosition;
        while (isMoving)
        {
            if (debug_kill)
            {
                RecieveDamage(1, Vector2.up);
                debug_kill = false;
            }
            yield return null;
        }
        animate = false;
    }
}
