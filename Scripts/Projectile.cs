using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	Collider2D col;
    Rigidbody2D rb;

    Light l;
    SpriteRenderer sr;

    Coroutine crt;

    float speedMemory = 7f;
	protected float speed = 7f;
    protected int damage = 1;

    public bool isInk = false;
    public bool isDiscoPellet = false;
    public bool isElectricBolt = false;

    protected bool targetShrimp = false;

    protected bool travelForward = false;

	protected void Awake(){
		col = GetComponent<Collider2D> ();
        rb = GetComponent<Rigidbody2D>();
        if (isDiscoPellet)
        {
            l = transform.GetChild(1).GetComponent<Light>();
            sr = transform.GetChild(2).GetComponent<SpriteRenderer>();
        }
	}

    public void Update()
    {
        if (travelForward)
        {
            rb.velocity = transform.right * speed;
            if (isDiscoPellet)
            {
                sr.transform.localEulerAngles = new Vector3(
                        sr.transform.localEulerAngles.x,
                        sr.transform.localEulerAngles.y,
                        sr.transform.localEulerAngles.z + 7
                    );
                if(crt==null)
                    StartCoroutine(StaticBuddy.ChangeColorWhileEnabled(sr, l));
            }
            if (StaticBuddy.IsOutOfBounds(transform))
            {
                travelForward = false;
                if (crt != null)
                {
                    StopCoroutine(crt);
                    crt = null;
                }
            }
        }
        else
        {
            gameObject.transform.localScale = new Vector3(1, 1, 1);
            gameObject.SetActive(false);
            if (isInk)
                Being.ReturnInk(gameObject);
            else if (isDiscoPellet)
                Being.ReturnDiscoPellet(gameObject);
            else if (isElectricBolt)
                Being.ReturnElectricBolt(gameObject);
            else
                Being.ReturnProjectile(gameObject);
        }
    }

    public void Launch(bool targetPlayerShrimp)
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, -1);
        speed = speedMemory;
        targetShrimp = targetPlayerShrimp;
        gameObject.SetActive(true);
        travelForward = true;
    }

    public void Launch(bool targetPlayerShrimp, float differentSpeed)
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, -1);
        speed = differentSpeed;
        targetShrimp = targetPlayerShrimp;
        gameObject.SetActive(true);
        travelForward = true;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (targetShrimp)
        {
            if (col.gameObject.tag.Equals(Shrimp.SHRIMP))
            {
                Shrimp e = col.gameObject.GetComponent<Shrimp>();
                e.RecieveDamage(
                    damage,
                    new Vector3(
                        Random.Range(-1, 1),
                        1,
                        0
                        )
                    );
                travelForward = false;
            }
        }
        else
        {
            if (col.gameObject.tag.Equals(Enemy.ENEMY))
            {
                Enemy e = col.gameObject.GetComponent<Enemy>();
                e.RecieveDamage(
                    damage,
                    new Vector3(
                        Random.Range(-1, 1),
                        1,
                        0
                        )
                    );
                travelForward = false;
            }
            else if (col.gameObject.tag.Equals("Menu"))
            {
                col.GetComponent<MenuRock>().Hit();
            }
        }
    }

}
