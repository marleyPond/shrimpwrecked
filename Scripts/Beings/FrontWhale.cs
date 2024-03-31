using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FrontWhale : Enemy {

    Shrimp shrimp;
    Transform bombSpawnTransform,
        mouthSpawnTransform;
    GameObject beam;
    SpriteRenderer eyes;

    bool isOnRightSide = false;
    float peekDistanceBase;

    List<int> lastStates;
    int lastStateMemory = 1;    //how many states do we keep from repeating?!
    List<int> possibleStates = new List<int>
    {
        STATE_PEEK_AND_FIRE,
        STATE_PEEK_AND_BEAM,
        STATE_TRAVERSE_SCREEN
    };

    int state = 0;
    static int STATE_IDLE = 0,
        STATE_PEEK_AND_FIRE = 1,
        STATE_PEEK_AND_BEAM = 2,
        STATE_TRAVERSE_SCREEN = 3;

    bool isThinking = false;
    bool isDeployingBombs = false;
    float deployBombsDelta = 1f;

    bool switchingDir = false;
    float switchDirBaseMS = 0.02f;
    int verDir = 1;

    int angleVeer = 15;
    float mouthDelta = 0.05f,
        volleyDelta = 0.75f,
        beamDelta = 0.05f;

    public void Awake()
    {
        cycleSpriteOnMovement = false;
        BaseInit();
        maxHealth = currentHealth = 30;
        bombSpawnTransform = transform.GetChild(0);
        mouthSpawnTransform = transform.GetChild(1);
        eyes = transform.GetChild(2).GetComponent<SpriteRenderer>();
        beam = transform.GetChild(3).gameObject;
        shrimp = FindObjectOfType<Shrimp>();
        director = FindObjectOfType<Director>();
        spriteFacesRightDefault = false;
        peekDistanceBase = GetComponent<CircleCollider2D>().radius / 2;
    }

    public void Start()
    {
        tag = ENEMY;
        OnDeathStart = EnemyOnDeathStart;
    }

    public override IEnumerator Move()
    {
        beam.SetActive(true);
        animator.transform.localScale = new Vector3(1, 1, 1);
        if (lastStates==null)
        {
            lastStates = new List<int>();
        }
        shrimp = FindObjectOfType<Shrimp>();
        if (!isThinking)
        {
            StartCoroutine(ProcessThoughts());
        }
        while (isMoving)
        {
            yield return null;
        }
        isDeployingBombs = false;
        isThinking = false;
        switchingDir = false;
        beam.SetActive(false);
        beam.transform.localScale = new Vector3(beam.transform.localScale.x, 0, beam.transform.localScale.z);
        isDeployingBombs = false;
        animate = false;
    }

    IEnumerator ProcessThoughts()
    {
        //TODO when states > 1, use lastState to keep from using the same thing over and over again
        isThinking = true;
        int nextState;
        while (isThinking)
        {
            isOnRightSide = UnityEngine.Random.Range(0, 2) == 0;
            nextState = possibleStates[UnityEngine.Random.Range(0, possibleStates.Count)];
            possibleStates.Remove(nextState);
            lastStates.Add(nextState);
            if (lastStates.Count > lastStateMemory)
            {
                int i = lastStates[0];
                lastStates.RemoveAt(0);
                possibleStates.Add(i);
            }
            if (nextState == STATE_PEEK_AND_FIRE)
                yield return StartCoroutine(PeekAndFire());
            else if (nextState == STATE_PEEK_AND_BEAM)
                yield return StartCoroutine(PeekAndBeam());
            else if(nextState==STATE_TRAVERSE_SCREEN)
                yield return StartCoroutine(TraverseScreen());
            

        }
    }

    IEnumerator PeekAndFire()
    {
        state = STATE_PEEK_AND_FIRE;
        yield return StartCoroutine(PeekIn());
        if(isThinking)
            yield return StartCoroutine(FireVolley());
        if (isThinking)
            yield return StartCoroutine(PeekOut());
        state = STATE_IDLE;
    }

    IEnumerator PeekAndBeam()
    {
        state = STATE_PEEK_AND_BEAM;
        yield return StartCoroutine(PeekIn());
        if (isThinking)
            yield return StartCoroutine(FireBeam());
        if (isThinking)
            yield return StartCoroutine(PeekOut());
        state = STATE_IDLE;
    }

    IEnumerator TraverseScreen()
    {
        state = STATE_TRAVERSE_SCREEN;
        bool sliding = true;
        isDeployingBombs = true;
        StartCoroutine(DeployBombs());
        if (isOnRightSide)
        {
            transform.localScale = new Vector3(
                    Mathf.Abs(transform.localScale.x),
                    transform.localScale.y,
                    transform.localScale.z
                );
            transform.position = director.GetSpawnPositionWhale(2);
            StaticBuddy.VerticalClamp(transform);
            while (isThinking && sliding)
            {
                rb.velocity = Vector3.left * speedMod;
                if (transform.position.x <= Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)).x)
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
            transform.position = director.GetSpawnPositionWhale(0) / 2;
            StaticBuddy.VerticalClamp(transform);
            while (isThinking && sliding)
            {
                rb.velocity = Vector3.right * speedMod;
                if (transform.position.x >= Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0)).x)
                    sliding = false;
                yield return null;
            }
        }
        state = STATE_IDLE;
        isDeployingBombs = false;
    }

    IEnumerator DeployBombs()
    {
        while (isDeployingBombs)
        {
            am.PlaySoundEffect(am.fxPathDrop);
            GameObject g = GetBomb();
            g.transform.position = bombSpawnTransform.position;
            g.GetComponent<Bomb>().SetLongFuse(UnityEngine.Random.Range(0.2f, 2.2f));
            g.SetActive(true);
            yield return new WaitForSeconds(deployBombsDelta);
        }
    }

    IEnumerator OpenMouth(Color eyeColor){
        Color c = eyes.color;
        for(int i = 0; i < 16; i++)
        {
            eyes.color = Color.Lerp(c, eyeColor, (i+1)/16);
            animator.SetIterator(i/2, i/2, i/2, 0);
            animator.UpdateSprite();
            yield return new WaitForSeconds(mouthDelta);
        }
    }

    IEnumerator CloseMouth()
    {
        Color c = eyes.color;
        for (int i = 15; i > -1; i--)
        {
            eyes.color = Color.Lerp(Color.white, c, 16/(i+1)/16);
            animator.SetIterator(i / 2, i / 2, i / 2, 0);
            animator.UpdateSprite();
            yield return new WaitForSeconds(mouthDelta);
        }
    }

    IEnumerator FireVolley()
    {
        yield return OpenMouth(Color.red);
        for (int i = 0; i < 12; i++)
        {
            am.PlaySoundEffect(am.fxPathGuitar);
            GameObject p = GetProjectile();
            p.transform.parent = null;
            p.transform.position = mouthSpawnTransform.transform.position;
            StaticBuddy.FaceOther(p.transform, shrimp.transform.position);
            p.transform.localScale = new Vector3(2f, 2f, 1);
            p.transform.Rotate(0, 0, Random.Range(-angleVeer, angleVeer));
            p.GetComponent<Projectile>().Launch(true, 3f);
            yield return new WaitForSeconds(volleyDelta);
        }
        yield return CloseMouth();
    }

    IEnumerator FireBeam()
    {
        yield return StartCoroutine(OpenMouth(Color.cyan));
        bool fireBeam = true;
        AudioSource src = am.PlaySoundEffect(am.fxPathBeam, true);
        StartCoroutine(StaticBuddy.ChangeColorWhileEnabled(
                beam.GetComponent<SpriteRenderer>(),
                beam.transform.GetChild(0).GetComponent<Light>()
            )
        );
        StartCoroutine(ProcessSineTravel());
        while (fireBeam)
        {
            while (beam.transform.localScale.y != 2)
            {
                beam.transform.localScale = new Vector3(
                        beam.transform.localScale.x,
                        Mathf.Min(beam.transform.localScale.y + 0.1f, 2),
                        1
                    );
                rb.velocity = transform.up * 0.5f * verDir;
                yield return new WaitForSeconds(beamDelta);
            }
            yield return new WaitForSeconds(1.5f);
            while (beam.transform.localScale.y != 0)
            {
                beam.transform.localScale = new Vector3(
                       beam.transform.localScale.x,
                       Mathf.Max(beam.transform.localScale.y - 0.1f, 0),
                       1
                   );
                rb.velocity = transform.up * 0.5f * verDir;
                yield return new WaitForSeconds(beamDelta);
            }
            fireBeam = false;
        }
        switchingDir = false;
        if (src != null)
            src.Stop();
        yield return StartCoroutine(CloseMouth());
    }

    IEnumerator ProcessSineTravel()
    {
        switchingDir = true;
        if (Random.Range(0, 2) == 0)
            verDir = -1;
        float ms = switchDirBaseMS;
        while (switchingDir)
        {
            verDir *= -1;
            yield return new WaitForSeconds(ms);
            ms = switchDirBaseMS * 2;
        }
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
            transform.position = director.GetSpawnPositionWhale(2);
            transform.position = new Vector3(transform.position.x + 2, transform.position.y, transform.position.z);
            StaticBuddy.VerticalClamp(transform);
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
            transform.position = director.GetSpawnPositionWhale(0)/2;
            StaticBuddy.VerticalClamp(transform);
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
                if (transform.position.x >= Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0)).x + 5)
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

    IEnumerator ProcessBombDeployment()
    {
        isDeployingBombs = true;
        while (isDeployingBombs)
        {
            yield return new WaitForSeconds(deployBombsDelta);
            if (isDeployingBombs)
            {
                am.PlaySoundEffect(am.fxPathDrop);
                GameObject g = GetBomb();
                g.transform.position = bombSpawnTransform.position;
                g.SetActive(true);
            }
        }
    }

    protected override IEnumerator OnEndOfDeathProcess()
    {
        isMoving = false;
        animate = false;
        yield return null;
        animator.gameObject.SetActive(true);
        col.enabled = true;
        FindObjectOfType<Director>().ReturnFrontWhaleToPool(this);
    }

    protected override void SetSpeedMod()
    {
        speedMod = 1.8f;
    }

}
