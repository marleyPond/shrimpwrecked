using UnityEngine;
using System.Collections;

public class Bomb : MonoBehaviour {

    bool longFuse = false;
    float force = 0.25f;

	void OnEnable()
    {
        StartCoroutine(FuseLit());
    }

    public void SetLongFuse(float force)
    {
        longFuse = true;
    }

    IEnumerator FuseLit()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (!longFuse)
            yield return new WaitForSeconds(3f);
        else
        {
            
            rb.gravityScale = 0.02f;
            rb.AddForce(Vector3.up * force);
            yield return new WaitForSeconds(6);
        }
        GameObject e = Being.GetExplosion();
        e.transform.position = transform.position;
        e.SetActive(true);
        gameObject.SetActive(false);
        Being.ReturnBomb(gameObject);
        longFuse = false;
        GetComponent<Rigidbody2D>().gravityScale = 0.01f;
    }
}
