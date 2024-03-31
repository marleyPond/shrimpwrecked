using UnityEngine;
using System.Collections;
using System;

public class Shrimp : Being {

    public static string SHRIMP = "Shrimp";

    Vector3 atRestAngle = new Vector3(0, -0.7f, 0);

    int lives = 20,
        livesMax;

    GameObject guitar;
    GameObject projectileSpawnPoint;

    NUI nui;

    float coolDownIterDelay = 0.02f;
    float desktopMoveRate = .5f;

    bool tapped = false;
    bool canFireProjectile = true;
    public bool canMove = false;

    void Awake()
    {
        guitar = transform.GetChild(0).gameObject;
        projectileSpawnPoint = transform.GetChild(1).gameObject;
        nui = FindObjectOfType<NUI>();
    }

    public void ResetLives()
    {
        lives = livesMax;
    }

    void Start()
    {
        BaseInit();
        gameObject.tag = SHRIMP;
        OnDeathStart = director.SlowThenRestoreTime;
        nui.SetLives(lives);
        BeginAnimate();
        invinsible = true;
        livesMax = lives;
    }

    void Update()
    {
        if (canMove && !isDying)
        {
            CheckForProjectileLaunch();
        }
    }

    void FixedUpdate()
    {
        if (canMove && !isDying)
        {
            Move();
        }
    }

    void Move()
    {

        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            MoveMobile();
        }
        else
        {
            float xval = 0;
            float yval = 0;
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            {
                yval = -desktopMoveRate;
            }
            else if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            {
                yval = desktopMoveRate;
            }
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                xval = -desktopMoveRate;
                if (goingRight && xval < 0)
                {
                    goingRight = false;
                    BeginTurn();
                }
            }
            else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                xval = desktopMoveRate;
                if (!(goingRight) && xval > 0)
                {
                    goingRight = true;
                    BeginTurn();
                }
            }
            rb.velocity = new Vector2(
                xval,
                yval
            ) * speedMod;
            StaticBuddy.ForceInsideBounds(transform);
        }
    }

    void MoveMobile()
    {
        float xval = Input.acceleration.x - atRestAngle.x;
        rb.velocity = new Vector2(
                xval,
                Input.acceleration.y - atRestAngle.y
            ) * speedMod;
        if (goingRight && xval < 0)
        {
            goingRight = false;
            BeginTurn();
        }
        else if (goingRight == false && xval > 0)
        {
            goingRight = true;
            BeginTurn();
        }
        StaticBuddy.ForceInsideBounds(transform);
    }

    void CheckForProjectileLaunch()
    {
        if (SystemInfo.deviceType != DeviceType.Handheld)
        {
            DesktopProjectileCheck();
            return;
        }
        if (Input.touchCount == 0)
        {
            tapped = false;
        }
        else if (Input.touchCount == 1 && canFireProjectile && !tapped)
        {
            tapped = true;
            StartCoroutine(ProcessProjectileCooldown());

            Touch t = Input.GetTouch(0);
            FireProjectileAtTarget(projectileSpawnPoint.transform.position,
                Camera.main.ScreenToWorldPoint(t.position), false);
        }
    }

    void DesktopProjectileCheck()
    {
        
        if (canFireProjectile && Input.GetMouseButtonDown(0))
        {
            StartCoroutine(ProcessProjectileCooldown());

            Vector2 t = (Vector2)Input.mousePosition;
            FireProjectileAtTarget(projectileSpawnPoint.transform.position,
                Camera.main.ScreenToWorldPoint(t), false);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag.Equals(Enemy.ENEMY))
        {
            Enemy e = col.gameObject.GetComponent<Enemy>();
            RecieveDamage(e.GetDamageValue(), Vector3.up);
        }
        else if(col.gameObject.tag.Equals("Beam")){
            RecieveDamageMaximum();
        }
    }

    IEnumerator ProcessProjectileCooldown()
    {
        canFireProjectile = false;
        for(float i = 0; i < 1.01f; i += 0.10f)
        {
            nui.SetCooldownPercentage(i);
            yield return new WaitForSeconds(coolDownIterDelay);
        }
        canFireProjectile = true;
    }

    protected override void SetSpeedMod()
    {
        speedMod = 6.0f;
    }

    protected override IEnumerator OnEndOfDeathProcess()
    {
        if (--lives <= 0 && Director.debug_shrimp_can_game_over)
        {
            lives = livesMax;
            currentHealth = maxHealth;
            director.EndGame();
            invinsible = true;
        }
        nui.SetLives(lives);
        transform.position = Vector3.zero;
        Flicker(OnHitFlickerCycles);
        yield return null;
    }

}
