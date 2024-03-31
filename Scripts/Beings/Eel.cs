using UnityEngine;
using System.Collections;

public class Eel : GenericFish
{

    new public void Awake()
    {
        BaseInit();
        spriteFacesRightDefault = false;
        loseOnRecieveDamageMethodOnActivation = false;
    }

    new public void Start()
    {
        tag = ENEMY;
        OnDeathStart = EnemyOnDeathStart;
        OnRecieveDamage = ExpressDistainInSuccumbingToDeath;
    }

    bool ExpressDistainInSuccumbingToDeath()
    {
        GameObject g = GetElectricShock();
        g.transform.position = transform.position;
        g.gameObject.SetActive(true);
        FireElectricBoltAtTargetEightWays(transform.position, true, 4f);
        return true;
    }

    protected override IEnumerator OnEndOfDeathProcess()
    {
        isMoving = false;
        animate = false;
        yield return null;
        if(animator!=null)
            animator.gameObject.SetActive(true);
        else
        {
            SpriteRenderer s = GetComponent<SpriteRenderer>();
            Color c = s.color;
            s.color = new Color(c.r, c.g, c.b, 1);
        }
        col.enabled = true;
        FindObjectOfType<Director>().ReturnEelToPool(this);
    }

}
