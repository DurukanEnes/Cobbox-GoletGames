using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cobbox
{
    public enum GameState { Ready, Playing, GameOver }

    // Owns the game state and score. Other scripts read it through GameManager.Instance.
    public class GameManager : MonoBehaviour
    {
        const string BestScoreKey = "BestScore";

        public static GameManager Instance { get; private set; }

        [SerializeField] float restartDelay = 0.5f;

        public GameState State { get; private set; } = GameState.Ready;
        public int Score { get; private set; }
        public int BestScore { get; private set; }
        public bool CanRestart => State == GameState.GameOver && Time.time - gameOverTime >= restartDelay;

        public event Action<GameState> StateChanged;
        public event Action<int> ScoreChanged;

        float gameOverTime;

        void Awake()
        {
            Instance = this;
            BestScore = PlayerPrefs.GetInt(BestScoreKey, 0);
            Application.targetFrameRate = 60;
        }

        public void StartGame()
        {
            if (State != GameState.Ready) return;
            SetState(GameState.Playing);
        }

        public void AddScore(int amount)
        {
            if (State != GameState.Playing) return;
            Score += amount;
            ScoreChanged?.Invoke(Score);
        }

        public void GameOver()
        {
            if (State != GameState.Playing) return;
            if (Score > BestScore)
            {
                BestScore = Score;
                PlayerPrefs.SetInt(BestScoreKey, BestScore);
                PlayerPrefs.Save();
            }
            gameOverTime = Time.time;
            SetState(GameState.GameOver);
            Debug.Log($"Game over - score: {Score}, best: {BestScore}");
        }

        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        void SetState(GameState state)
        {
            State = state;
            StateChanged?.Invoke(state);
        }
    }
}
