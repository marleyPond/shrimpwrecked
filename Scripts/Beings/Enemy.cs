using UnityEngine;
using System.Collections;

public abstract class Enemy : Being {

    static public string ENEMY = "Enemy";

    public bool isMoving = false;
    protected bool cycleSpriteOnMovement = true;
    public int moveDir = 0;     //0,1,2,3->right,up,left,down

    protected int damage = 1;

    public abstract IEnumerator Move();

    protected void DetermineDirectionToFace()
    {
        int faceVal = 1;
        if (!spriteFacesRightDefault)
            faceVal = -1;
        if (moveDir == 0)
            transform.localScale = new Vector3(
                   faceVal * Mathf.Abs(transform.localScale.x),
                   transform.localScale.y,
                   transform.localScale.z
                );
        else if (moveDir == 2)
            transform.localScale = new Vector3(
                   faceVal * -1 * Mathf.Abs(transform.localScale.x),
                   transform.localScale.y,
                   transform.localScale.z
                );
        else
        {
            int mod = 1;
            if (Random.Range(0, 1) == 0)
                mod = -1;
            transform.localScale = new Vector3(
                   faceVal * mod * Mathf.Abs(transform.localScale.x),
                   transform.localScale.y,
                   transform.localScale.z
                );
        }
    }

    protected IEnumerator EnemyOnDeathStart()
    {
        if (loseOnRecieveDamageMethodOnActivation)
            OnRecieveDamage = null;
        isMoving = false;
        yield return null;
    }

    public void BeginMoving(int moveDir)
    {
        this.moveDir = moveDir;
        isMoving = true;
        if(cycleSpriteOnMovement)
            BeginAnimate();
        StartCoroutine(Move());
    }

    public int GetDamageValue()
    {
        return damage;
    }

}
