using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// UIManager is in charge of listening to game events and update the UI
/// </summary>
public class UIManager : MonoBehaviour
{
	[Header("Gameplay")] 
	[SerializeField] UICanvasSwitcher gameplayCanvasSwitcher;
	[SerializeField] UIAnimatedText scoreText;
	[SerializeField] UIAnimatedText remainingMovesText;
	
	[Header("Game Over")]
	[SerializeField] UICanvasSwitcher gameOverCanvasSwitcher;
	[SerializeField] UIAnimatedText gameOverScoreText;
	[SerializeField] UIAnimatedText gameOverText;
	[SerializeField] Button playButton;

	void Awake()
	{
		playButton.onClick.AddListener(() =>
		{
			playButton.interactable = false;
			SceneManager.LoadScene("Main");
		});
	}

	IEnumerator Start()
	{
		yield return new WaitForSeconds(.1f);
		gameplayCanvasSwitcher.Show();
	}

	void OnEnable()
	{
		GameState.NewLineEvent += OnNewLineEvent;
		GameState.RemainingMovesUpdatedEvent += OnRemainingMovesUpdatedEvent;
		GameState.GameOverEvent += OnGameOverEvent;
	}
	
	void OnDisable()
	{
		GameState.NewLineEvent -= OnNewLineEvent;
		GameState.RemainingMovesUpdatedEvent -= OnRemainingMovesUpdatedEvent;
		GameState.GameOverEvent -= OnGameOverEvent;
	}
	void OnGameOverEvent()
	{
		gameplayCanvasSwitcher.Hide();
		gameOverCanvasSwitcher.Show();
		gameOverText.NewTextAnimated("Game Over");
	}

	void OnNewLineEvent(bool success, List<BlockCoordinate> blocksInTheLine, int newScore)
	{
		if (success)
		{
			
			gameOverScoreText.NewTextAnimated(newScore.ToString("N0"));
			
			if (newScore > PlayerPrefs.GetInt("max-score"))
			{
				PlayerPrefs.SetInt("max-score", newScore);
				gameOverScoreText.NewTextAnimated(newScore.ToString("N0") + "\t New Record!");
			}
			else
			{
				scoreText.NewTextAnimated(newScore.ToString("N0"));	
			}
			
		}
	}

	void OnRemainingMovesUpdatedEvent(int remainingMoves)
	{
		string moves = "";
		for (int i = 0; i < remainingMoves; i++)
		{
			if (i % 5 == 0 && i != 0)
				moves += " ";
			moves += "/";
			
		}

		remainingMovesText.NewTextAnimated(moves);
	}
}
