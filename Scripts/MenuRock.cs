using UnityEngine;
using System.Collections;

public class MenuRock : MonoBehaviour {

	public int health_current = 3;
    public int health_total = 3;

    public delegate void OnNoHealth();

    public OnNoHealth NoHealthFunc;

    SpriteRenderer sr;

    void Awake()
    {
        sr = transform.GetComponent<SpriteRenderer>();
    }

    public void Hit()
    {
        if (--health_current <= 0)
        {
            NoHealthFunc();
        }else
        {
            StartCoroutine(MiniFlicker());
        }
    }

    IEnumerator MiniFlicker()
    {
        for(int i = 1; i < 11; i++)
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(.01f);
            sr.color = Color.white;
            yield return new WaitForSeconds(.01f);
        }
        sr.color = Color.white;
    }
}