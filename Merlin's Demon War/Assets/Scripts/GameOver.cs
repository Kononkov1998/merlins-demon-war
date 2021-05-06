using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public Text scoreText;
    public Text killsText;

    private void Awake()
    {
        scoreText.text = $"Score: {GameController.instance.score}";
        killsText.text = $"Demons killed: {GameController.instance.kills}";
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
