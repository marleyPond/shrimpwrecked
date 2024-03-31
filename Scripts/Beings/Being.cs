using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Being : MonoBehaviour {

    protected delegate IEnumerator CustomCoroutine();
    protected CustomCoroutine OnDeathStart;
    public delegate bool BooleanMethod();
    public BooleanMethod OnRecieveDamage = null;

    protected bool soundOnDeath = true;
    protected bool flickerCanToggleCollider = true;

    protected bool loseOnRecieveDamageMethodOnActivation = true;

    static protected List<GameObject> projectilePool;
    static protected List<GameObject> spikePool;
    static protected List<GameObject> bombPool;
    static protected List<GameObject> explosionPool;
    static protected List<GameObject> electricShockPool;
    static protected List<GameObject> inkPool;
    static protected List<GameObject> discoPool;
    static protected List<GameObject> electricBoltPool;

    static protected GameObject projectilePrefab;

    static protected AudioManager am;
    static protected Director director;

    protected bool goingRight = true;
    protected bool isTurning = false;
    protected bool spriteFacesRightDefault = true;
    protected bool animate = false;
    protected bool isDying = false;
    protected bool isFlickering = false;
    protected float turnRate = 0.01f;//0.1f;
    protected float animateFramerate = 0.30f;
    protected float originalScaleX;

    protected int flickerCycleMin = 0,
        flickerCycleMax = 120,
        flickerCycleDefault = 60,
        OnHitFlickerCycles = 30,
        OnDeathFlickerCycles = 90;

    protected int currentHealth = 1;
    protected int maxHealth = 1;
    protected int currentAttack = 1;
    protected float speedMod;

    public bool invinsible = false;

    protected Sprite_Animator animator;
    protected SpriteRenderer r;
    public List<Sprite> spriteList;
    protected Rigidbody2D rb;
    protected Collider2D col;

    protected abstract void SetSpeedMod();
    protected abstract IEnumerator OnEndOfDeathProcess();

    protected void BaseInit()
    {
        if (projectilePool == null) { 
            projectilePool = new List<GameObject>();
            spikePool = new List<GameObject>();
            bombPool = new List<GameObject>();
            explosionPool = new List<GameObject>();
            electricShockPool = new List<GameObject>();
            inkPool = new List<GameObject>();
            discoPool = new List<GameObject>();
            electricBoltPool = new List<GameObject>();

            am = FindObjectOfType<AudioManager>();
            director = FindObjectOfType<Director>();
        }
        originalScaleX = transform.localScale.x;
        if (spriteList != null && spriteList.Count != 0)
        {
            animator = (Sprite_Animator)Instantiate(Resources.Load("Prefabs\\Animation\\Sprite_Animator", typeof(Sprite_Animator)));
            animator.gameObject.layer = 10;
            animator.AddLayer(spriteList, Color.white);
            animator.sprite_layers[0].gameObject.layer = 10;
            animator.transform.parent = transform;
            animator.transform.localPosition = Vector2.zero;
            animator.SetIteratorAll(0, 0, spriteList.Count - 1);
            animator.UpdateSprite();
        }
        SetSpeedMod();
        r = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        if (r != null && animator!=null)
            r.enabled = false;
    }

    public void RecieveDamageMaximum()
    { 
        Die(new Vector3(
                    Random.Range(-1, 1),
                    1,
                    0
                    ));
        
    }

    public void RecieveDamage(int damage, Vector3 angle)
    {
        if (!invinsible)
        {
            if (OnRecieveDamage == null || OnRecieveDamage())
            {
                if (loseOnRecieveDamageMethodOnActivation)
                    OnRecieveDamage = null;
                currentHealth -= damage;
                if (currentHealth == 0)
                    Die(angle);
                else
                {
                    am.PlaySoundEffect(am.fxPathHit);
                    Flicker(OnHitFlickerCycles);
                }
            }
        }
    }

    protected void FireProjectileAtTarget(Vector3 originate, Vector3 target, bool targetShrimp)
    {
        GameObject p = GetProjectile();
        p.transform.parent = null;
        p.transform.position = originate;
        StaticBuddy.FaceOther(p.transform, target);
        p.GetComponent<Projectile>().Launch(targetShrimp);
        am.PlaySoundEffect(am.fxPathGuitar);
    }

    protected void FireProjectileAtTarget(Vector3 originate, Vector3 target, bool targetShrimp, float speedMod)
    {
        GameObject p = GetProjectile();
        p.transform.parent = null;
        p.transform.position = originate;
        StaticBuddy.FaceOther(p.transform, target);
        p.GetComponent<Projectile>().Launch(targetShrimp, speedMod);
        am.PlaySoundEffect(am.fxPathGuitar);
    }

    protected void FireProjectileEightWays(Vector3 origin, bool targetShrimp, float speedMod)
    {
        GameObject s = null;
        for (int i = 0; i < 359; i = i + 45)
        {
            s = GetProjectile();
            s.transform.position = origin;
            s.transform.rotation = Quaternion.Euler(0, 0, i);
            s.gameObject.SetActive(true);
            s.GetComponent<Projectile>().Launch(targetShrimp, speedMod);
        }
        am.PlaySoundEffect(am.fxPathGuitar);
    }

    protected void FireSpikeAtTarget(Vector3 originate, Vector3 target, bool targetShrimp)
    {
        GameObject p = GetSpike();
        p.transform.parent = null;
        p.transform.position = originate;
        StaticBuddy.FaceOther(p.transform, target);
        p.GetComponent<Spike>().Launch(targetShrimp);
        am.PlaySoundEffect(am.fxPathSpike);
    }

    protected void FireSpikeAtTarget(Vector3 originate, Vector3 target, bool targetShrimp, float speedMod)
    {
        GameObject p = GetSpike();
        p.transform.parent = null;
        p.transform.position = originate;
        StaticBuddy.FaceOther(p.transform, target);
        p.GetComponent<Spike>().Launch(targetShrimp, speedMod);
        am.PlaySoundEffect(am.fxPathSpike);
    }

    protected void FireSpikeAtTargetEightWays(Vector3 originate, bool targetShrimp, float speedMod)
    {
        GameObject s = null;
        for (int i = 0; i < 359; i = i + 45)
        {
            s = GetSpike();
            s.transform.position = originate;
            s.transform.rotation = Quaternion.Euler(0, 0, i);
            s.gameObject.SetActive(true);
            s.GetComponent<Spike>().Launch(targetShrimp, speedMod);
        }
        am.PlaySoundEffect(am.fxPathSpike);
    }

    protected void FireDiscoPelletAtTarget(Vector3 originate, Vector3 target, bool targetShrimp)
    {
        GameObject p = GetDiscoPellet();
        p.transform.parent = null;
        p.transform.position = originate;
        StaticBuddy.FaceOther(p.transform, target);
        p.GetComponent<Projectile>().Launch(targetShrimp);
        am.PlaySoundEffect(am.fxPathDisco);
    }

    protected void FireDiscoPelletAtTarget(Vector3 originate, Vector3 target, bool targetShrimp, float speedMod)
    {
        GameObject p = GetDiscoPellet();
        p.transform.parent = null;
        p.transform.position = originate;
        StaticBuddy.FaceOther(p.transform, target);
        p.GetComponent<Projectile>().Launch(targetShrimp, speedMod);
        am.PlaySoundEffect(am.fxPathDisco);
    }

    protected void FireDiscoPelletAtTargetEightWays(Vector3 originate, bool targetShrimp, float speedMod)
    {
        GameObject s = null;
        for (int i = 0; i < 359; i = i + 45)
        {
            s = GetDiscoPellet();
            s.transform.position = transform.position;
            s.transform.rotation = Quaternion.Euler(0, 0, i);
            s.gameObject.SetActive(true);
            s.GetComponent<Projectile>().Launch(true, speedMod);
        }
        am.PlaySoundEffect(am.fxPathDisco);
    }

    protected void FireElectricBoltAtTarget(Vector3 originate, Vector3 target, bool targetShrimp)
    {
        GameObject p = GetElectricBolt();
        p.transform.parent = null;
        p.transform.position = originate;
        StaticBuddy.FaceOther(p.transform, target);
        p.GetComponent<Projectile>().Launch(targetShrimp);
        am.PlaySoundEffect(am.fxPathElectric);
    }

    protected void FireElectricBoltAtTarget(Vector3 originate, Vector3 target, bool targetShrimp, float speedMod)
    {
        GameObject p = GetElectricBolt();
        p.transform.parent = null;
        p.transform.position = originate;
        StaticBuddy.FaceOther(p.transform, target);
        p.GetComponent<Projectile>().Launch(targetShrimp, speedMod);
        am.PlaySoundEffect(am.fxPathElectric);
    }

    protected void FireElectricBoltAtTargetEightWays(Vector3 originate, bool targetShrimp, float speedMod)
    {
        GameObject s = null;
        for (int i = 0; i < 359; i = i + 45)
        {
            s = GetElectricBolt();
            s.transform.position = transform.position;
            s.transform.rotation = Quaternion.Euler(0, 0, i);
            s.gameObject.SetActive(true);
            s.GetComponent<Projectile>().Launch(true, speedMod);
        }
        am.PlaySoundEffect(am.fxPathElectric);
    }

    protected void FireInkAtTarget(Vector3 originate, Vector3 target, bool targetShrimp)
    {
        GameObject p = GetInk();
        p.transform.parent = null;
        p.transform.position = originate;
        StaticBuddy.FaceOther(p.transform, target);
        p.GetComponent<Projectile>().Launch(targetShrimp);
        am.PlaySoundEffect(am.fxPathInk);
    }

    protected void FireInkAtTarget(Vector3 originate, Vector3 target, bool targetShrimp, float speedMod)
    {
        GameObject p = GetInk();
        p.transform.parent = null;
        p.transform.position = originate;
        StaticBuddy.FaceOther(p.transform, target);
        p.GetComponent<Projectile>().Launch(targetShrimp, speedMod);
        am.PlaySoundEffect(am.fxPathInk);
    }

    protected void FireInkAtTargetEightWays(Vector3 originate, bool targetShrimp, float speedMod)
    {
        GameObject s = null;
        for (int i = 0; i < 359; i = i + 45)
        {
            s = GetInk();
            s.transform.position = transform.position;
            s.transform.rotation = Quaternion.Euler(0, 0, i);
            s.gameObject.SetActive(true);
            s.GetComponent<Projectile>().Launch(true, speedMod);
        }
        am.PlaySoundEffect(am.fxPathInk);
    }

    public bool DisguiseDiscovered()
    {
        MimicOctopus m = FindObjectOfType<MimicOctopus>();
            m.discoveredSpawnPos = transform.position;
        m.Discovered();
        return true;
    }


    public GameObject GetProjectile()
    {
        GameObject ret = null;
        if (projectilePool.Count != 0)
        {
            ret = projectilePool[0];
            projectilePool.RemoveAt(0);
        }else
        {
            ret = (GameObject)Instantiate(Resources.Load("Prefabs\\Projectile"));
        }
        return ret;
    }

    public GameObject GetSpike()
    {
        GameObject ret = null;
        if (spikePool.Count != 0)
        {
            ret = spikePool[0];
            spikePool.RemoveAt(0);
        }
        else
        {
            ret = (GameObject)Instantiate(Resources.Load("Prefabs\\Spike"));
        }
        return ret;
    }

    public static void ReturnProjectile(GameObject g)
    {
        Spike s = g.GetComponent<Spike>();
        if (s != null)
            spikePool.Add(g);
        else
            projectilePool.Add(g);
    }

    public GameObject GetBomb()
    {
        GameObject ret = null;
        if (bombPool.Count != 0)
        {
            ret = bombPool[0];
            bombPool.RemoveAt(0);
        }
        else
        {
            ret = (GameObject)Instantiate(Resources.Load("Prefabs\\Bomb", typeof(GameObject)));
        }
        return ret;
    }

    public static void ReturnBomb(GameObject g)
    {
        bombPool.Add(g);
    }

    public static GameObject GetExplosion()
    {
        GameObject ret = null;
        if (explosionPool.Count != 0)
        {
            ret = explosionPool[0];
            explosionPool.RemoveAt(0);
        }
        else
        {
            ret = (GameObject)Instantiate(Resources.Load("Prefabs\\Explosion", typeof(GameObject)));
        }
        return ret;
    }

    public static GameObject GetElectricShock()
    {
        GameObject ret = null;
        if (electricShockPool.Count != 0)
        {
            ret = electricShockPool[0];
            electricShockPool.RemoveAt(0);
        }
        else
        {
            ret = (GameObject)Instantiate(Resources.Load("Prefabs\\ElectricShock", typeof(GameObject)));
        }
        return ret;
    }

    public static GameObject GetInk()
    {
        GameObject ret = null;
        if (inkPool.Count != 0)
        {
            ret = inkPool[0];
            inkPool.RemoveAt(0);
        }
        else
        {
            ret = (GameObject)Instantiate(Resources.Load("Prefabs\\Ink", typeof(GameObject)));
            ret.GetComponent<Projectile>().isInk = true;
        }
        return ret;
    }

    public static GameObject GetDiscoPellet()
    {
        GameObject ret = null;
        if (discoPool.Count != 0)
        {
            ret = discoPool[0];
            discoPool.RemoveAt(0);
        }
        else
        {
            ret = (GameObject)Instantiate(Resources.Load("Prefabs\\DiscoShot", typeof(GameObject)));
            ret.GetComponent<Projectile>().isDiscoPellet = true;
        }
        return ret;
    }

    public static GameObject GetElectricBolt()
    {
        GameObject ret = null;
        if (electricBoltPool.Count != 0)
        {
            ret = electricBoltPool[0];
            electricBoltPool.RemoveAt(0);
        }
        else
        {
            ret = (GameObject)Instantiate(Resources.Load("Prefabs\\ElectricBolt", typeof(GameObject)));
            ret.GetComponent<Projectile>().isElectricBolt = true;
        }
        return ret;
    }

    public static void ReturnExplosion(GameObject g)
    {
        explosionPool.Add(g);
    }

    public static void ReturnElectricShock(GameObject g)
    {
        electricShockPool.Add(g);
    }

    public static void ReturnInk(GameObject g)
    {
        inkPool.Add(g);
    }

    public static void ReturnDiscoPellet(GameObject g)
    {
        discoPool.Add(g);
    }

    public static void ReturnElectricBolt(GameObject g)
    {
        electricBoltPool.Add(g);
    }

    protected void BeginTurn()
    {
        if (!isTurning) {
            isTurning = true;
            StartCoroutine(ProcessTurn());
        }
    }

    IEnumerator ProcessTurn()
    {
        float x;

        while (isTurning)
        {
            x = transform.localScale.x;
            if (goingRight)
            {
                    if (spriteFacesRightDefault) {
                        x = Mathf.Min(x + turnRate, originalScaleX);
                        if (x == originalScaleX)
                            isTurning = false;
                    }else
                    {
                        x = Mathf.Max(x - turnRate, -originalScaleX);
                        if (x == -originalScaleX)
                            isTurning = false;
                    }
            }
            else
            {
                    if (spriteFacesRightDefault)
                    {
                        x = Mathf.Max(x - turnRate, -originalScaleX);
                        if (x == -originalScaleX)
                            isTurning = false;
                    }else
                    {
                        x = Mathf.Min(x + turnRate, originalScaleX);
                        if (x == originalScaleX)
                            isTurning = false;
                    }
            }
            if (x == 0)
                x += 0.01f;
            transform.localScale = new Vector3(
                        x,
                        transform.localScale.y,
                        transform.localScale.z
                    );
            if (isTurning)
                yield return null;
        }
        yield return null;
    }

    public void BeginAnimate()
    {
        if (animator != null)
        {
            animate = true;
            StartCoroutine(ProcessAnimation());
        }
    }

    IEnumerator ProcessAnimation()
    {
        while (animate)
        {
            animator.UpdateIterate();
            yield return new WaitForSeconds(animateFramerate);
        }
        yield return null;
    }

    protected void Flicker(int cycles)
    {
        isFlickering = true;
        StartCoroutine(ProcessFlicker(cycles));
    }

    IEnumerator ProcessFlicker(int cycles)
    {
        if (col != null && flickerCanToggleCollider)
            col.enabled = false;
        if (cycles <= flickerCycleMin || cycles > flickerCycleMax)
            cycles = flickerCycleDefault;
        float a = 1;
        if (r != null)
            a = r.color.a;         
        while (isFlickering)
        {
            
            if (animator != null)
            {
                if (!animator.isActiveAndEnabled)
                {
                    animator.gameObject.SetActive(true);
                    yield return new WaitForSeconds(0.06f);
                }
                else
                {
                    animator.gameObject.SetActive(false);
                    yield return new WaitForSeconds(0.02f);
                }
            }
            else
            {
                if (r.color.a!=a)
                {
                    r.color = new Color(r.color.r, r.color.g, r.color.b, a);
                    yield return new WaitForSeconds(0.06f);
                }
                else
                {
                    r.color = new Color(r.color.r, r.color.g, r.color.b, 0.01f);
                    yield return new WaitForSeconds(0.02f);
                }
            }
            if (--cycles < 0)
                isFlickering = false;
        }
        if (animator == null)
            r.color = new Color(r.color.r, r.color.g, r.color.b, a);
        else
            animator.gameObject.SetActive(true);
        if (col != null && flickerCanToggleCollider)
            col.enabled = true;
    }
   

    void Die(Vector3 angle)
    {
        if(soundOnDeath)
            am.PlaySoundEffect(am.fxPathDie);
        isDying = true;
        if (OnDeathStart != null && gameObject.activeSelf) 
        {
            StartCoroutine(OnDeathStart());
            Flicker(OnDeathFlickerCycles);
        }
        StartCoroutine(ProcessDeath(angle));
    }

    IEnumerator ProcessDeath(Vector3 angle)
    {
        rb.gravityScale = 0.7f;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = 0;
        rb.AddForce(angle * 75f);
        float rotation = Random.Range(-12f, 12f);
        while (isDying)
        {
            rb.rotation += rotation;
            if (StaticBuddy.IsOutOfBoundsExtendedBottomOnly(transform))
                isDying = false;
            yield return null;
        }
        currentHealth = maxHealth;
        rb.gravityScale = 0;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = 0;
        transform.eulerAngles = Vector3.zero;
        isFlickering = false;
        StartCoroutine(OnEndOfDeathProcess());
    }

    

}
