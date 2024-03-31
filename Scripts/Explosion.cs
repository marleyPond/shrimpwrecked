using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

    ParticleSystem ps;

    int damage = 1;

    static AudioManager am;

    void Awake()
    {
        if (am == null)
            am = FindObjectOfType<AudioManager>();
    }

	void OnEnable()
    {
        if (ps == null)
            ps = GetComponent<ParticleSystem>();
        ps.Simulate(0);
        ps.Play();
        StartCoroutine(Explode());
    }

    IEnumerator Explode()
    {
        am.PlaySoundEffect(am.fxPathExplosion);
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
        if (gameObject.name.Equals("ElectricShock(Clone)"))
            Being.ReturnElectricShock(gameObject);
        else
            Being.ReturnExplosion(gameObject);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag.Equals("Enemy") ||
            col.gameObject.tag.Equals("Shrimp"))
        {
            Being b = col.GetComponent<Being>();
            b.RecieveDamage(
                damage,
                new Vector3(
                    Random.Range(-1, 1),
                    1,
                    0
                    )
                );
        }
    }
}
