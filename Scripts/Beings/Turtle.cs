using UnityEngine;
using System.Collections;

public class Turtle : GenericFish {

    new public void Awake()
    {
        BaseInit();
        spriteFacesRightDefault = false;
    }

    new public void Start()
    {
        tag = ENEMY;
        OnDeathStart = EnemyOnDeathStart;
        currentHealth = maxHealth = 2;
    }

    protected override IEnumerator OnEndOfDeathProcess()
    {
        isMoving = false;
        animate = false;
        yield return null;
        animator.gameObject.SetActive(true);
        col.enabled = true;
        FindObjectOfType<Director>().ReturnTurtleToPool(this);
    }

}
