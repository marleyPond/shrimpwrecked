using UnityEngine;
using System.Collections;
using System;

public class Jellyfish : Enemy
{
    Vector2 dir;

    Light glow;

    public void Awake()
    {
        BaseInit();
        invinsible = true;
        glow = transform.GetChild(0).GetComponent<Light>();
    }

    public void Start()
    {
        tag = ENEMY;
        OnDeathStart = EnemyOnDeathStart;
    }


    void OnEnable()
    {
        Color c = ColorHelper.GetColorRandomNoShades();
        c = new Color(c.r, c.g, c.b, 0.25f);
        animator.sprite_layers[0].SetColorNormal(c);
        glow.color = c;
        float scale = UnityEngine.Random.Range(0.75f, 1.25f);
        transform.localScale = new Vector3(scale, scale, 1);
    }

    public override IEnumerator Move()
    {

        while (isMoving)
        {
            dir = director.GetScrollInfo();
            dir = -1 * dir;
            dir.Normalize();
            rb.velocity = dir * speedMod;
            yield return null;
            if (StaticBuddy.IsOutOfBoundsExtended(transform))
                RecieveDamageMaximum();
        }
        animate = false;
    }

    protected override IEnumerator OnEndOfDeathProcess()
    {
        isMoving = false;
        animate = false;
        yield return null;
        animator.gameObject.SetActive(true);
        col.enabled = true;
        FindObjectOfType<Director>().ReturnJellyfishToPool(this);
    }

    protected override void SetSpeedMod()
    {
        speedMod = 0.25f;
    }
}
