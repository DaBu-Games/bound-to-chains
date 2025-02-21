using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CheckForChains : MonoBehaviour
{
    public bool isColliding { get; private set; }

    public List<Transform> chainsInHitbox = new List<Transform>(); // Chains in the trigger zone
    public Transform currentChain { get; private set; } // The chain the player is climbing towards

    public Player player; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Add chain to the list when it enters the hitbox
        if (collision.CompareTag("Chain"))
        {
            chainsInHitbox.Add(collision.transform);
        }
        else if ( collision.CompareTag("Ball") && player.GetPlayerState() is ClimbingState )
        {
            isColliding = false;
            player.FinishClimb();
        }
        else if (collision.CompareTag("Platform"))
        {
            isColliding = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Remove chain from the list when it exits the hitbox
        if (collision.CompareTag("Chain"))
        {
            chainsInHitbox.Remove(collision.transform);

        }
        else if (collision.CompareTag("Platform") && isColliding && player.GetPlayerState() is not ClimbingState && !player.HasNoExcludeLayers())
        {
            isColliding = false;
            player.ResetExludeLayers();
        }
        else if (collision.CompareTag("Ground") && player.GetPlayerState() is ClimbingState)
        {
            isColliding = false;
            player.FinishClimb();
        }
    }

    public void SetChain(Transform highestChain)
    {
        currentChain = highestChain;
    }
}
