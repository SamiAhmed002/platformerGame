using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public GameObject linkedPortal;  // Reference to the other portal
    public float teleportCooldown = 2f;  // 2-second cooldown
    private bool isCooldownActive = false;  // Track if cooldown is active

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision occured");
        // Check if the player entered the portal and if cooldown is not active
        if (other.CompareTag("Player") && !isCooldownActive)
        {
            Debug.Log("Player entered the portal!");  // Log to check if portal triggers
            StartCoroutine(TeleportWithCooldown(other));
        }
        // Check if the enemy entered the portal
        else if (other.CompareTag("Enemy") && !isCooldownActive)
        {
            Debug.Log("Enemy entered the portal!");
            StartCoroutine(TeleportEnemyWithCooldown(other));
        }
    }

    IEnumerator TeleportEnemyWithCooldown(Collider enemy)
    {
        Debug.Log("Teleporting enemy, starting cooldown...");  // Log teleport initiation
        // Set cooldown active
        isCooldownActive = true;

        // Move the enemy to the linked portal's position
        enemy.transform.position = linkedPortal.transform.position + linkedPortal.transform.forward * 2f;
        enemy.GetComponent<CRAZYEnemyMovement>().hasTeleported = true;

        Rigidbody rb = enemy.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true; // Disable gravity after teleport
        }

        Debug.Log("Enemy teleported to: " + enemy.transform.position);  // Log the enemy's new position

        // Also set the cooldown on the linked portal to avoid immediate back-and-forth teleport
        linkedPortal.GetComponent<Portal>().isCooldownActive = true;

        // Wait for the cooldown duration (2 seconds)
        yield return new WaitForSeconds(teleportCooldown);

        // Cooldown ends
        isCooldownActive = false;
        linkedPortal.GetComponent<Portal>().isCooldownActive = false;

        Debug.Log("Cooldown ended, portals are ready.");  // Log when cooldown ends
    }

    IEnumerator TeleportWithCooldown(Collider player)
    {
        Debug.Log("Teleporting player, starting cooldown...");  // Log teleport initiation
        // Set cooldown active
        isCooldownActive = true;

        CharacterController playerController = player.GetComponent<CharacterController>();

        // Disable the CharacterController to avoid any physics issues during teleport
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        // Move the player to the linked portal's position
        player.transform.position = linkedPortal.transform.position + linkedPortal.transform.forward * 2f;

        Debug.Log("Player teleported to: " + player.transform.position);  // Log the player's new position

        // Re-enable the CharacterController
        if (playerController != null)
        {
            playerController.enabled = true;
        }

        // Also set the cooldown on the linked portal to avoid immediate back-and-forth teleport
        linkedPortal.GetComponent<Portal>().isCooldownActive = true;

        // Wait for the cooldown duration (2 seconds)
        yield return new WaitForSeconds(teleportCooldown);

        // Cooldown ends
        isCooldownActive = false;
        linkedPortal.GetComponent<Portal>().isCooldownActive = false;

        Debug.Log("Cooldown ended, portals are ready.");  // Log when cooldown ends
    }
}
