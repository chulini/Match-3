using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// In charge of listening to game events and update the UI
/// </summary>
public class UIManager : MonoBehaviour
{
	[Header("Gameplay")] 
	[SerializeField] UICanvasSwitcher gameplayCanvasSwitcher;
	[SerializeField] UIAnimatedText scoreText;
	[SerializeField] UIAnimatedText remainingMovesText;
	
	[Header("Game Over")]
	[SerializeField] UICanvasSwitcher gameOverCanvasSwitcher;
	[SerializeField] UIAnimatedText gameOverText;
	[SerializeField] Button playButton;
	
	GameState _gameState;

	void Awake()
	{
		playButton.onClick.AddListener(() =>
		{
			playButton.interactable = false;
			SceneManager.LoadScene("Main");
		});
	}

	public void Init(GameState gameState)
	{
		_gameState = gameState;
	}

	IEnumerator Start()
	{
		yield return new WaitForSeconds(.5f);
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
		Debug.Log("GAME OVER");
		gameplayCanvasSwitcher.Hide();
		gameOverCanvasSwitcher.Show();
		gameOverText.NewTextAnimated("Game Over");
	}
	
	void OnNewLineEvent(bool success, List<BlockCoordinate> blocksInTheLine)
	{
		if (success)
		{
			scoreText.NewTextAnimated( _gameState.score.ToString("N0"));
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
