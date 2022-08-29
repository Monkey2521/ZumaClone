using UnityEngine;
using UnityEngine.UI;

public class ScoreCounter : MonoBehaviour, IGameStartHandler, IGameOverHandler, IScoreUpdateHandler
{
    [Header("Debug settings")]
    [SerializeField] private bool _isDebug;

    [Header("Settings")]
    [SerializeField] private Text _scoreText;
    [SerializeField] private GameObject _starGO;
    [SerializeField] private Image _selfImage;

    private int _score;
    public int Score => _score;

    private void OnEnable()
    {
        EventBus.Subscribe(this);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe(this);    
    }

    public void OnGameStart()
    {
        _selfImage.enabled = true;
        _starGO.SetActive(true);
        _scoreText.gameObject.SetActive(true);

        _score = 0;
        UpdateUI();
    }

    public void OnGameOver()
    {
        _selfImage.enabled = false;
        _starGO.SetActive(false);
        _scoreText.gameObject.SetActive(false);
    }

    public void OnScoreUpdate(int score)
    {
        _score += score;

        UpdateUI();
    }

    private void UpdateUI() => _scoreText.text = _score.ToString();
}
