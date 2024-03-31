using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NUI : MonoBehaviour
{

    Text labelLives, labelLivesAmount, labelMessage;
    Image cooldownBackground, cooldownForeground;
    bool displayingMessage = false;
    Text creditsTitle, creditsPeople;

    Image catawampusLogo;
    Image logoBackdrop;

    float halfDelay = 2;

    string[] creditTitles = new string[]
    {
        "Initial Concept",
        "Design",
        "Artwork",
        "Programming",
        "Music",
        "Dedicated To",
        "Thanks For Playing \n Shrimp Wrecked"
    };

    string[] creditNames = new string[]
    {
        "<color=cyan>Mar</color><color=orange>ley</color>\nSpicolian Wonder",
        "<color=cyan>Mar</color><color=orange>ley</color>",
        "<color=cyan>Mar</color><color=orange>ley</color>\nSpicolian Wonder",
        "<color=cyan>Mar</color><color=orange>ley</color>",
        "<color=cyan>Mar</color><color=orange>ley</color>",
        "Sam\nJacey\nMr. Charlie & Junepie\nHeather",
        ""
    };

    void Awake()
    {
        labelLives = transform.GetChild(0).GetComponent<Text>();
        labelLivesAmount = transform.GetChild(1).GetComponent<Text>();
        cooldownBackground = transform.GetChild(2).GetComponent<Image>();
        cooldownForeground = transform.GetChild(3).GetComponent<Image>();
        labelMessage = transform.GetChild(4).GetComponent<Text>();
        labelMessage.enabled = false;
        logoBackdrop = transform.GetChild(5).GetComponent<Image>();
        catawampusLogo = logoBackdrop.transform.GetChild(0).GetComponent<Image>();
        creditsTitle = transform.GetChild(6).GetComponent<Text>();
        creditsPeople = transform.GetChild(7).GetComponent<Text>();
    }

    public IEnumerator RockLogoAndRevealStartScreen()
    {
        AudioManager am = FindObjectOfType<AudioManager>();
        am.PlayCatawampusJingle();
        catawampusLogo.CrossFadeAlpha(255f, 30, true);
        yield return new WaitForSeconds(4f);
        catawampusLogo.CrossFadeAlpha(1f, 1.5f, true);
        yield return new WaitForSeconds(2f);
        logoBackdrop.gameObject.SetActive(false);
        FindObjectOfType<Shrimp>().canMove = true;
        am.PlaySongsRandom();
    }

    public void KillLogoScreen()
    {
        logoBackdrop.gameObject.SetActive(false);
    }

    public void SetCooldownPercentage(float state)
    {
        cooldownForeground.transform.localScale = new Vector3(
                state,
                cooldownForeground.transform.localScale.y,
                cooldownForeground.transform.localScale.z
            );
    }

    public void SetLives(int lives)
    {
        labelLivesAmount.text = lives.ToString();
    }

    public float DisplayMessage(string message)
    {
        if (!displayingMessage)
        {
            StartCoroutine(ProcessMessage(message));
            return 6;
        }
        return 0;
    }

    IEnumerator ProcessMessage(string message)
    {
        displayingMessage = true;
        labelMessage.text = message;
        labelMessage.enabled = true;
        labelMessage.CrossFadeAlpha(255, halfDelay, true);
        yield return new WaitForSeconds(halfDelay + 2);
        labelMessage.CrossFadeAlpha(1, halfDelay, true);
        yield return new WaitForSeconds(halfDelay);
        labelMessage.enabled = false;
        displayingMessage = false;
    }

    public IEnumerator ProcessCredits()
    {
        FindObjectOfType<Shrimp>().canMove = false;

        logoBackdrop.gameObject.SetActive(true);
        catawampusLogo.enabled = false;
        int size = creditTitles.Length;
        for(int i = 0; i < size; i++)
        {
            creditsTitle.text = creditTitles[i];
            creditsPeople.text = creditNames[i];
            creditsTitle.color = new Color(creditsTitle.color.r, creditsTitle.color.g, creditsTitle.color.b, 0.1f);
            creditsPeople.color = new Color(creditsPeople.color.r, creditsPeople.color.g, creditsPeople.color.b, 0.1f);
            creditsTitle.gameObject.SetActive(true);
            creditsPeople.gameObject.SetActive(true);
            creditsTitle.CrossFadeAlpha(255, 20, true);
            creditsPeople.CrossFadeAlpha(255, 20, true);
            yield return new WaitForSeconds(halfDelay + 2);
            creditsTitle.CrossFadeAlpha(1, halfDelay, true);
            creditsPeople.CrossFadeAlpha(1, halfDelay, true);
            yield return new WaitForSeconds(halfDelay);
        }
        creditsTitle.gameObject.SetActive(false);
        creditsPeople.gameObject.SetActive(false);
        catawampusLogo.enabled = true;
        catawampusLogo.CrossFadeAlpha(255f, 15, true);
        yield return new WaitForSeconds(2f);
        catawampusLogo.CrossFadeAlpha(1f, 1.5f, true);
        yield return new WaitForSeconds(2f);
        catawampusLogo.enabled = false;
        logoBackdrop.gameObject.SetActive(false);
        FindObjectOfType<Shrimp>().canMove = true;
        yield return new WaitForSeconds(halfDelay);
        FindObjectOfType<Director>().EndGame();
        FindObjectOfType<Shrimp>().ResetLives();
    }

}