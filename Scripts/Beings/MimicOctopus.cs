using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MimicOctopus : Enemy {

    bool debug_start_as_mimic_octopus = false;      //false

    Shrimp shrimp;
    int secondaryHealthCurrent = 12,
        secondaryHealthMax = 12;

    bool isOnRightSide = false;
    float peekDistanceBase;

    bool isThinking = false,
        wasDiscovered = false;
    bool isActing = false;

    List<int> lastStates;
    int lastStateMemory = 1;    //how many states do we keep from repeating?!
    List<int> possibleStates = new List<int>
    {
        STATE_FADE_ATTACK,
        STATE_FISH_BLEND,
        STATE_REGULAR_OCTOPUS_BLEND
    };

    static int STATE_IDLE = 0,
        STATE_FADE_ATTACK = 1,
        STATE_FISH_BLEND = 2,
        STATE_REGULAR_OCTOPUS_BLEND = 3;

    Light glow;
    float normalIntensity;

    public Vector3 discoveredSpawnPos = Vector3.zero;
    bool pretendingToBeJellyfish = true;

    public void Awake()
    {
        cycleSpriteOnMovement = false;
        flickerCanToggleCollider = false;
        BaseInit();
        maxHealth = currentHealth = 16;
        shrimp = FindObjectOfType<Shrimp>();
        director = FindObjectOfType<Director>();
        spriteFacesRightDefault = false;
        peekDistanceBase = GetComponent<BoxCollider2D>().size.x / 2;
        OnRecieveDamage = HandleJellyfishDamage;
        loseOnRecieveDamageMethodOnActivation = false;
        glow = transform.GetChild(0).GetComponent<Light>();
        normalIntensity = glow.intensity;
    }

    public void Start()
    {
        tag = ENEMY;
        OnDeathStart = EnemyOnDeathStart;
    }

    //returning true results in normal damage calculations occuring
    protected bool HandleJellyfishDamage()
    {
        if (--secondaryHealthCurrent == 0)
        {
            StartCoroutine(RevealTrueSelf());
            director.RequestMessageDisplay("Mimic Octopus");
            return false;
        }else if(secondaryHealthCurrent>0)
        {
            Flicker(OnHitFlickerCycles);
            return false;
        }
        return true;
 
    }

    public override IEnumerator Move()
    {
        if (debug_start_as_mimic_octopus)
            pretendingToBeJellyfish = false;
        if (lastStates == null)
        {
            lastStates = new List<int>();
        }
        animator.transform.localScale = new Vector3(1, 1, 1);
        shrimp = FindObjectOfType<Shrimp>();
        if (!isThinking)
        {
            StartCoroutine(ProcessThoughts());
        }
        while (isMoving)
        {
            yield return null;
        }
        isThinking = false;
        animate = false;
    }

    IEnumerator ProcessThoughts()
    {
        isThinking = true;
        int nextState, holder;
        while (isThinking)
        {
            if (pretendingToBeJellyfish)
            {
                isOnRightSide = Random.Range(0, 2) == 0;
                yield return StartCoroutine(PeekAndSpawnJellyfishWalls());
            }
            else
            {
                if (!isActing)
                {
                    yield return new WaitForSeconds(3.4f);
                    isOnRightSide = Random.Range(0, 2) == 0;
                    nextState = possibleStates[Random.Range(0, possibleStates.Count)];
                    possibleStates.Remove(nextState);
                    lastStates.Add(nextState);
                    if (lastStates.Count > lastStateMemory)
                    {
                        holder = lastStates[0];
                        lastStates.RemoveAt(0);
                        possibleStates.Add(holder);
                    }
                    if (nextState == STATE_FADE_ATTACK)
                    {
                        StartCoroutine(ProcessFadeAttack(true));
                    }
                    else if (nextState == STATE_REGULAR_OCTOPUS_BLEND)
                    {
                        StartCoroutine(AttackFromGroupOfOctopi());
                    }
                    else//must be STATE_FISH_BLEND
                        StartCoroutine(AttackFromSchoolOfFish());
                }
                if (wasDiscovered)
                {
                    wasDiscovered = false;
                    transform.position = discoveredSpawnPos;
                    StartCoroutine(ProcessFadeAttack(false));
                }
                yield return null;
            }
        }
    }

    IEnumerator PeekAndSpawnJellyfishWalls()
    {
        yield return StartCoroutine(PeekIn());
        if (pretendingToBeJellyfish&&isThinking)
            yield return StartCoroutine(SpawnJellyfishWalls());
        if (pretendingToBeJellyfish && isThinking)
            yield return StartCoroutine(PeekOut());
    }

    IEnumerator RevealTrueSelf()
    {
        pretendingToBeJellyfish = false;
        invinsible = true;
        isActing = true;
        int size = animator.sprite_layers[0].sprites.Count-1;
        for (int i = 0; i < size; i++)
        {
            animator.UpdateIterate();
            yield return new WaitForSeconds(0.5f);
        }
        invinsible = false;
        yield return StartCoroutine(FadeOut());
        isActing = false;
    }

    IEnumerator SpawnJellyfishWalls()
    {
        int spawnLayers = Random.Range(7, 13);
        int missingPos = Random.Range(1, 5);
        for (int i = 0; i < spawnLayers; i++)
        {
            director.SpawnJellyfishWall(transform.position.x, missingPos);
            yield return new WaitForSeconds(1);
            if (!pretendingToBeJellyfish)
                i = spawnLayers;
            else
              missingPos = Mathf.Clamp(missingPos + Random.Range(-1, 2), 1, 5);            
        }
    }

    IEnumerator ProcessFadeAttack(bool newPos)
    {
        isActing = true;
        yield return StartCoroutine(FadeInAndAttack(newPos));
        if (isThinking)
            yield return StartCoroutine(FadeOut());
        isActing = false;
    }

    IEnumerator FadeInAndAttack(bool newPos)
    {
        SpriteRenderer sr = animator.sprite_layers[0].sr;
        Color c = sr.color;
        if (newPos)
        {
            transform.position = StaticBuddy.GetRandomInScreenPositionPadded(0.2f);
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        }
        for(float i = 0.01f; i < 1.01f; i = i + 0.01f)
        {
            glow.intensity = Mathf.Lerp(0, normalIntensity, i);
            sr.color = new Color(c.r, c.g, c.b, i);
            yield return new WaitForSeconds(0.02f);
        }
        col.enabled = true;
        ShootInk();
        yield return new WaitForSeconds(1.5f);
    }

    IEnumerator FadeOut()
    {
        col.enabled = false;
        SpriteRenderer sr = animator.sprite_layers[0].GetComponent<SpriteRenderer>();
        Color c = sr.color;
        for (float i = 1; i > 0 ; i = i - 0.01f)
        {
            glow.intensity = Mathf.Lerp(0, normalIntensity, i);
            sr.color = new Color(c.r, c.g, c.b, i);
            yield return new WaitForSeconds(0.01f);
        }
    }

    public void Discovered()
    {
        wasDiscovered = true;
    }

    IEnumerator AttackFromSchoolOfFish()
    {
        col.enabled = false;
        isActing = true;
        yield return StartCoroutine(director.MimicOctopusSpawnGenericFishSchool());
        if (isThinking)
            yield return new WaitForSeconds(4);
        isActing = false;
    }

    IEnumerator AttackFromGroupOfOctopi()
    {
        col.enabled = false;
        isActing = true;
        yield return StartCoroutine(director.MimicOctopusSpawnOctopiSchool());
        if (isThinking)
            yield return new WaitForSeconds(4);
        isActing = false;
    }

    void ShootInk()
    {
        FireInkAtTarget(transform.position, shrimp.transform.position, true, 3f);
    }

    IEnumerator PeekIn()
    {
        bool sliding = true;
        if (isOnRightSide)
        {
            transform.localScale = new Vector3(
                    Mathf.Abs(transform.localScale.x),
                    transform.localScale.y,
                    transform.localScale.z
                );
            transform.position = director.GetSpawnPositionWhale(2) / 2;
            transform.position = new Vector3(transform.position.x + 2, transform.position.y, transform.position.z);
            transform.position = VerticalClamp();
            while (isThinking && sliding)
            {
                rb.velocity = Vector3.left * speedMod;
                if (transform.position.x <= Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0)).x)
                    sliding = false;
                yield return null;
            }
        }
        else
        {
            transform.localScale = new Vector3(
                -Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );
            transform.position = director.GetSpawnPositionWhale(0) / 4;
            transform.position = VerticalClamp();
            while (isThinking && sliding)
            {
                rb.velocity = Vector3.right * speedMod;
                if (transform.position.x >= Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)).x)
                    sliding = false;
                yield return null;
            }
        }
        rb.velocity = Vector3.zero;
    }

    IEnumerator PeekOut()
    {
        bool sliding = true;
        if (isOnRightSide)
        {
            while (isThinking && sliding)
            {
                rb.velocity = Vector3.right * speedMod;
                if (transform.position.x >= Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0)).x + 2.5f)
                    sliding = false;
                yield return null;
            }
        }
        else
        {
            while (isThinking && sliding)
            {
                rb.velocity = Vector3.left * speedMod;
                if (transform.position.x <= Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)).x - 5)
                    sliding = false;
                yield return null;
            }
        }
    }

    Vector3 VerticalClamp()
    {
        return transform.position = new Vector3(
                    transform.position.x,
                    Mathf.Clamp(
                            transform.position.y,
                            Camera.main.ViewportToWorldPoint(new Vector3(0, 0.2f, 0)).y,
                            Camera.main.ViewportToWorldPoint(new Vector3(0, 0.8f, 0)).y
                        ),
                    transform.position.z
                );
    }

    protected override IEnumerator OnEndOfDeathProcess()
    {
        isMoving = false;
        animate = false;
        yield return null;
        Color c = animator.sprite_layers[0].sr.color;
        animator.sprite_layers[0].sr.color = new Color(c.r, c.g, c.b, 255);
        animator.sprite_layers[0].SetIterator(0, 0, animator.sprite_layers.Count - 1);
        pretendingToBeJellyfish = true;
        secondaryHealthCurrent = secondaryHealthMax;
        animator.gameObject.SetActive(true);
        col.enabled = true;
        FindObjectOfType<Director>().ReturnMimicOctopusToPool(this);
    }

    protected override void SetSpeedMod()
    {
        speedMod = 1.2f;
    }
}
