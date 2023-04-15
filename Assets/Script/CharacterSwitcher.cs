using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class CharacterSwitcher : MonoBehaviour
{
    public List<GameObject> characters;
    private int activeCharacterIndex = 0;
    private bool Grounded = true;
    public CinemachineVirtualCamera CMcamera;

    public int ActiveCharacterIndex
    {
        get { return activeCharacterIndex; }
    }

    private void Start()
    {
        for (int i = 0; i < characters.Count; i++)
        {
            if (i == activeCharacterIndex)
            {
                characters[i].GetComponent<CharacterController2D>().enabled = true;
            }
            else
            {
                characters[i].GetComponent<CharacterController2D>().enabled = false;
            }
        }
    }

    private void Update()
    {
        Grounded = characters[activeCharacterIndex].GetComponent<CharacterController2D>().isGrounded;
        if (Input.GetKeyDown(KeyCode.Q) && Grounded)
        {
            SwitchCharacter(-1);
           
        }
        else if (Input.GetKeyDown(KeyCode.E) && Grounded)
        {
            SwitchCharacter(1);
           
        }
    }

    private void SwitchCharacter(int direction)
    {
        // Update the active character index
        int newIndex = activeCharacterIndex + direction;
        if (newIndex < 0)
        {
            newIndex = characters.Count - 1;
        }
        else if (newIndex >= characters.Count)
        {
            newIndex = 0;
        }

        // Swap the positions of the active character and the new character
        Vector3 tempPosition = characters[activeCharacterIndex].transform.position;
        characters[activeCharacterIndex].transform.position = characters[newIndex].transform.position;
        characters[newIndex].transform.position = tempPosition;

        // Disable the current active character's controller
        characters[activeCharacterIndex].GetComponent<CharacterController2D>().enabled = false;

        // Enable the new active character's controller
        characters[newIndex].GetComponent<CharacterController2D>().enabled = true;

        CMcamera.LookAt = characters[newIndex].transform;
        CMcamera.Follow = characters[newIndex].transform;

        // Update the active character index
        activeCharacterIndex = newIndex;
    }
 

    public List<GameObject> GetCurrentCharacterOrder()
    {
        return new List<GameObject>(characters);
    }
}
