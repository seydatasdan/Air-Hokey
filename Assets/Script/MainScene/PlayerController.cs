using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public List<PlayerMovement> Players = new List<PlayerMovement>(); // List to store references to PlayerMovement components

    public GameObject AiPlayer; // Reference to the AI player game object

    private void Start()
    {
        if (GameValues.IsMultiplayer)
        {
            // Enable PlayerMovement component and disable AiScript component for multiplayer mode
            AiPlayer.GetComponent<PlayerMovement>().enabled = true;
            AiPlayer.GetComponent<AiScript>().enabled = false;
        }
        else
        {
            // Enable AiScript component and disable PlayerMovement component for single-player mode
            AiPlayer.GetComponent<PlayerMovement>().enabled = false;
            AiPlayer.GetComponent<AiScript>().enabled = true;
        }
    }

    private bool isMouseButtonDown = false;
    private bool isMouseButtonHeld = false;

    private void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isMouseButtonDown = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            isMouseButtonDown = false;
            isMouseButtonHeld = false;
        }
        if (Input.GetMouseButton(0))
        {
            isMouseButtonHeld = true;
        }

        if (isMouseButtonDown)
        {
            // Convert mouse position to world position
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            foreach (var player in Players)
            {
                if (player.LockedFingerID == null)
                {
                    // Check if the player's collider overlaps with the mouse position
                    if (player.PlayerCollider.OverlapPoint(mouseWorldPos))
                    {
                        player.LockedFingerID = 0; // Set the locked finger ID to 0 when the mouse is used
                    }
                }
                else if (player.LockedFingerID == 0)
                {
                    // Move the player to the mouse position if the locked finger ID is 0 (indicating mouse input)
                    player.MoveToPosition(mouseWorldPos);
                }
            }
        }
    }
}
