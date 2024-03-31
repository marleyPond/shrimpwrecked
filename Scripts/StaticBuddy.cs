using UnityEngine;
using System.Collections;

public static class StaticBuddy {

    public static void ForceInsideBounds(Transform t)
    {
        Vector3 wtv = Camera.main.WorldToViewportPoint(t.position);
        wtv.x = Mathf.Clamp(wtv.x, 0.05f, 0.95f);
        wtv.y = Mathf.Clamp(wtv.y, 0.05f, 0.95f);
        t.position = Camera.main.ViewportToWorldPoint(wtv);
    }
    
    public static Vector3 GetRandomInScreenPositionPadded(float viewportPad)
    {
        return Camera.main.ViewportToWorldPoint(
            new Vector3(
                Random.Range(viewportPad, 1 - viewportPad),
                Random.Range(viewportPad, 1 - viewportPad),
                0
                )
            );
    }

    public static bool IsOutOfBounds(Transform t)
    {
        Vector3 wtv = Camera.main.WorldToViewportPoint(t.position);
        return !(wtv.x > 0 && wtv.y > 0 && wtv.x < 1 && wtv.y < 1);
    }

    public static bool IsOutOfBoundsExtended(Transform t)
    {
        Vector3 wtv = Camera.main.WorldToViewportPoint(t.position);
        return wtv.x < -0.1f || wtv.y < -0.1f || wtv.x > 1.1f || wtv.y > 1.1f;
    }

    public static bool IsOutOfBoundsWhale(Transform t)
    {
        Vector3 wtv = Camera.main.WorldToViewportPoint(t.position);
        return wtv.x < -2f || wtv.y < -2f || wtv.x > 2f || wtv.y > 2f;
    }

    public static bool IsOutOfBoundsExtendedBottomOnly(Transform t)
    {
        Vector3 wtv = Camera.main.WorldToViewportPoint(t.position);
        return wtv.y <= -0.1f;
    }

    public static bool IsOutOfBoundsExtendedTopOnly(Transform t)
    {
        Vector3 wtv = Camera.main.WorldToViewportPoint(t.position);
        return wtv.y >= 1.1f;
    }

    public static bool IsOutOfBoundsExtendedRightOnly(Transform t)
    {
        Vector3 wtv = Camera.main.WorldToViewportPoint(t.position);
        return wtv.x >= 1.1f;
    }

    public static bool IsOutOfBoundsExtendedLeftOnly(Transform t)
    {
        Vector3 wtv = Camera.main.WorldToViewportPoint(t.position);
        return wtv.x <= -0.1f;
    }

    public static void FaceOther(Transform t, Vector3 lookPosition)
    {
        Vector3 diff = lookPosition - t.position;
        diff.Normalize();
        float zRot = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        t.rotation = Quaternion.Euler(0f, 0f, zRot);
    }

    public static void VerticalClamp(Transform t)
    {
        t.position = new Vector3(
                    t.position.x,
                    Mathf.Clamp(
                            t.position.y,
                            Camera.main.ViewportToWorldPoint(new Vector3(0, 0.2f, 0)).y,
                            Camera.main.ViewportToWorldPoint(new Vector3(0, 0.8f, 0)).y
                        ),
                    t.position.z
                );
    }

    public static IEnumerator ChangeColorWhileEnabled(SpriteRenderer sr)
    {
        float delta = 0.1f;
        Color c = sr.color;
        float r = Mathf.Clamp(c.r + Random.Range(-delta, delta), 0, 1f),
               g = Mathf.Clamp(c.g + Random.Range(-delta, delta), 0, 1f),
               b = Mathf.Clamp(c.b + Random.Range(-delta, delta), 0, 1f);
        int rand;
        while (sr.enabled && sr.gameObject.activeSelf)
        {
            rand = Random.Range(0, 3);
            if (rand == 0)
                r = Mathf.Clamp(r + Random.Range(-delta, delta), 0, 1f);
            else if (rand == 1)
                g = Mathf.Clamp(g + Random.Range(-delta, delta), 0, 1f);
            else
                b = Mathf.Clamp(b + Random.Range(-delta, delta), 0, 1f);

            sr.color = new Color(r, g, b);
            yield return new WaitForSeconds(0.02f);
        }
    }

    public static IEnumerator ChangeColorWhileEnabled(SpriteRenderer sr, Light l)
    {
        float delta = 0.1f;
        Color c = sr.color;
        float r = Mathf.Clamp(c.r + Random.Range(-delta, delta), 0, 1f),
               g = Mathf.Clamp(c.g + Random.Range(-delta, delta), 0, 1f),
               b = Mathf.Clamp(c.b + Random.Range(-delta, delta), 0, 1f);
        int rand;
        while (sr.enabled && sr.gameObject.activeSelf)
        {
            rand = Random.Range(0, 3);
            if (rand == 0)
                r = Mathf.Clamp(r + Random.Range(-delta, delta), 0, 1f);
            else if (rand == 1)
                g = Mathf.Clamp(g + Random.Range(-delta, delta), 0, 1f);
            else
                b = Mathf.Clamp(b + Random.Range(-delta, delta), 0, 1f);

            sr.color = new Color(r, g, b);
            l.color = new Color(r, g, b);
            yield return new WaitForSeconds(0.25f);
        }
    }

}
