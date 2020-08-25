using UnityEngine;
using System.Collections;

/// <summary>
/// FlockFollower scripts uses LookAt() function to follow wherever Flock goes.
/// </summary>
public class FlockFollower : MonoBehaviour
{
	/// <summary>
	/// Flock to follow.
	/// </summary>
	public Flock flock;
	public bool flockInfoEnabled = true;
	
	internal int boardX = 10;
	internal int boardY = 10;
	internal int boardWidth = 200;
	internal int boardHeight = 120;
	
	/// <summary>
	/// Looks at the Flock.
	/// </summary>
	void LateUpdate()
	{
		if (this.flock != null) 
		{
			transform.LookAt(flock.flockCenter + flock.transform.position);
		}
	}
	
	/// <summary>
	/// Update Flock information.
	/// </summary>
	void OnGUI()
	{
		if (this.flock != null && this.flockInfoEnabled)
		{
			this.ShowAll(this.flock);
		}
	} 
	
	/// <summary>
	/// Resets GUI positions.
	/// </summary>
	void ResetPosition()
	{
		this.boardX = 10;
		this.boardY = 10;
		this.boardWidth = 200;
		this.boardHeight = 120;
	}
	
	/// <summary>
	/// Shows all the Flock information.
	/// </summary>
	/// <param name="flock">
	/// A <see cref="Flock"/>
	/// </param>
	void ShowAll(Flock flock)
	{
		ResetPosition();
		DisplayFlockInfo(flock);
	}
	
	/// <summary>
	/// Draws GUI parts.
	/// </summary>
	/// <param name="flock">
	/// A <see cref="Flock"/>
	/// </param>
	void DisplayFlockInfo(Flock flock)
	{
		GUI.Box(new Rect(boardX, boardY, boardWidth, boardHeight), "Flock Information");
		boardX += 5;
		boardY += 20;
		boardWidth = 190;
		boardHeight = 20;
		GUI.Label(new Rect(boardX, boardY, boardWidth, boardHeight), "Position: " + flock.transform.position);
		boardY += 20;
		GUI.Label(new Rect(boardX, boardY, boardWidth, boardHeight), "Center: " + flock.GetFlockCenter());
		boardY += 20;
		GUI.Label(new Rect(boardX, boardY, boardWidth, boardHeight), "Velocity: " + flock.GetFlockVelocity());
		boardY += 20;
		GUI.Label(new Rect(boardX, boardY, boardWidth, boardHeight), "Leader: " + flock.flockLeader.position);
	}
}
