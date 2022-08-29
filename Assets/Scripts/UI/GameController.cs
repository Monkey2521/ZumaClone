using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour, IGameOverHandler
{
    [SerializeField] private ScoreCounter _counter;
    [SerializeField] private Text _gameOverText;
    [SerializeField] private GameObject _gameOverMenu;
    [SerializeField] private GameObject _startButton;

    private void OnEnable()
    {
        EventBus.Subscribe(this);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe(this);    
    }

    public void StartGame()
    {
        _gameOverMenu.SetActive(false);
        _startButton.SetActive(false);
        
        EventBus.Publish<IGameStartHandler>(handler => handler.OnGameStart());
    }

    public void OnGameOver()
    {
        _gameOverMenu.SetActive(true);
        _gameOverText.text = "Game over!\nTotal score: " + _counter.Score.ToString(); 
    }

    public void Exit()
    {
        Application.Quit();
    }
}
