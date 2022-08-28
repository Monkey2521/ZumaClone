using UnityEngine;
using UnityEngine.UI;

public class ScoreCounter : MonoBehaviour, IGameStartHandler, IGameOverHandler, IScoreUpdateHandler
{
    [Header("Debug settings")]
    [SerializeField] private bool _isDebug;

    [Header("Settings")]
    [SerializeField] private Text _scoreText;

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
        _score = 0;
        UpdateUI();
    }

    public void OnGameOver()
    {

    }

    public void OnScoreUpdate(int score)
    {
        _score += score;

        UpdateUI();
    }

    private void UpdateUI() => _scoreText.text = _score.ToString();
}
