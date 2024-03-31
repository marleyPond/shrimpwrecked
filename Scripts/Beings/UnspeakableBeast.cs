using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnspeakableBeast : Enemy {

    Shrimp shrimp;

    Transform pivot;

    bool isThinking = false;
    bool spawnMiniAnglerFish = false;

    Light glow;

    GameObject bobble;
    List<GameObject> orifi;

    public void Awake()
    {
        BaseInit();
        animator.transform.localScale = new Vector3(1, 1, 1);
        glow = (Light)transform.GetChild(0).GetComponent<Light>();
        bobble = (GameObject)transform.GetChild(1).gameObject;
        orifi = new List<GameObject>();
        orifi.Add((GameObject)transform.GetChild(2).gameObject);
        orifi.Add((GameObject)transform.GetChild(3).gameObject);
        orifi.Add((GameObject)transform.GetChild(4).gameObject);
        pivot = transform.GetChild(5);
        spriteFacesRightDefault = false;
        currentHealth = maxHealth = 4;
    }

    public void Start()
    {
        tag = ENEMY;
        OnDeathStart = EnemyOnDeathStart;
        bobble.GetComponent<AnglerFishOrifice>().eightShot = true;
    }

    public override IEnumerator Move()
    {
        shrimp = FindObjectOfType<Shrimp>();
        yield return new WaitForSeconds(0.1f);
        if (orifi[0].GetComponent<AnglerFishOrifice>().owner == null)
        {
            AnglerFishOrifice afo;
            for (int i = 0; i < orifi.Count; i++)
            {
                afo = orifi[i].GetComponent<AnglerFishOrifice>();
                afo.BeginMoving(0);
                afo.owner = this;
                afo.gameObject.SetActive(true);
            }
            afo = bobble.GetComponent<AnglerFishOrifice>();
            afo.BeginMoving(0);
            afo.owner = this;
            afo.gameObject.SetActive(true);
        }
        else
        {
            for (int i = 0; i < orifi.Count; i++)
                orifi[i].GetComponent<AnglerFishOrifice>().BeginMoving(0);
            bobble.GetComponent<AnglerFishOrifice>().BeginMoving(0);
        }
        if (!isThinking)
            StartCoroutine(ProcessThoughts());
        StartCoroutine(StaticBuddy.ChangeColorWhileEnabled(
            bobble.GetComponent<SpriteRenderer>(),
            bobble.transform.GetChild(0).GetComponent<Light>()
                )            
            );
        goingRight = !goingRight;
        BeginTurn();
        StartCoroutine(SpawnMiniAnglerFish());
        while (isMoving)
        {
            ApproachShrimp();
            yield return null;
        }
        spawnMiniAnglerFish = false;
        isThinking = false;
        animate = false;
    }

    IEnumerator SpawnMiniAnglerFish()
    {
        if (!spawnMiniAnglerFish)
        {
            spawnMiniAnglerFish = true;
            while (spawnMiniAnglerFish)
            {
                yield return new WaitForSeconds(Random.Range(5, 10));
                if(spawnMiniAnglerFish)
                    FindObjectOfType<Director>().SpawnMiniAnglerFish(transform.position);
            }
        }
    }

    void ApproachShrimp()
    {
        StaticBuddy.FaceOther(pivot, shrimp.transform.position);
        rb.velocity = pivot.right * speedMod;
        if (goingRight && rb.velocity.x < 0)
        {
            goingRight = false;
            BeginTurn();
        }
        else if (goingRight == false && rb.velocity.x > 0)
        {
            goingRight = true;
            BeginTurn();
        }

    }

    protected IEnumerator ProcessThoughts()
    {
        isThinking = true;
        while (isThinking)
        {
            yield return null;
        }
    }

    protected override IEnumerator OnEndOfDeathProcess()
    {
        isMoving = false;
        animate = false;
        yield return null;
        bobble.transform.parent = transform;
        bobble.transform.localPosition = bobble.GetComponent<AnglerFishOrifice>().originalLoc;
        for(int i = 0; i < orifi.Count; i++)
        {
            orifi[i].transform.parent = transform;
            orifi[i].transform.localPosition = orifi[i].GetComponent<AnglerFishOrifice>().originalLoc;
        }
        animator.gameObject.SetActive(true);
        FindObjectOfType<Director>().ReturnUnspeakableBeastToPool(this);
    }

    protected override void SetSpeedMod()
    {
        speedMod = 0.5f;
    }
}
