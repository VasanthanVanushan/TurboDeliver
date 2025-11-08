using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Delivery : MonoBehaviour
{
    bool hasPackage;
    [SerializeField] float delay = 1f;
    int score;

    [Header("UI References")]
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text timerText;
    [SerializeField] TMP_Text gameOverText;

    [Header("Game Settings")]
    [SerializeField] float gameDuration = 50f; // total seconds
    [SerializeField] int winScore = 50;        // score needed to win

    float timeRemaining;
    bool gameActive = true;

    void Start()
    {
        score = 0;
        timeRemaining = gameDuration;
        gameOverText.gameObject.SetActive(false);
        UpdateScoreUI();
        UpdateTimerUI();
    }

    void Update()
    {
        if (gameActive)
        {
            // Countdown timer
            timeRemaining -= Time.deltaTime;
            UpdateTimerUI();

            if (timeRemaining <= 0)
            {
                EndGame(false); // lose by time out
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!gameActive) return; // ignore if game ended

        if (collision.CompareTag("Package") && !hasPackage)
        {
            hasPackage = true;
            GetComponent<ParticleSystem>().Play();
            Destroy(collision.gameObject, delay);
        }

        if (collision.CompareTag("Customer") && hasPackage)
        {
            hasPackage = false;
            GetComponent<ParticleSystem>().Stop();
            Destroy(collision.gameObject, delay);
            score += 10;
            UpdateScoreUI();

            // Win check
            if (score >= winScore)
            {
                EndGame(true);
            }
        }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score.ToString();
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int displayTime = Mathf.CeilToInt(Mathf.Max(timeRemaining, 0));
            timerText.text = "Time: " + displayTime.ToString();

            // Optional: turn red when under 10s
            timerText.color = (displayTime <= 10) ? Color.red : Color.white;
        }
    }

    void EndGame(bool won)
    {
        gameActive = false;
        timeRemaining = 0;
        UpdateTimerUI();

        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);

            if (won)
                gameOverText.text = " You Won! \nFinal Score: " + score+ "\n Time Remaining: "+timeRemaining;
            else
                gameOverText.text = " Time's Up!\nFinal Score: " + score;
        }

        // Restart scene after short delay
        Invoke(nameof(RestartGame), 7f);
    }

    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
