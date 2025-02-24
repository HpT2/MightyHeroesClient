using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerComponent
{
    public Character OwnerCharacter;
    public Vector3 MoveDirection;
    public float MoveSpeed;
    public PlayerControllerComponent(Character character)
    {
        OwnerCharacter = character;
    }

    public void BindInput()
    {
        Ingame IngameUI = UIManager.Instance.GetUIByType<Ingame>();
        if (IngameUI)
        {
            IngameUI.OnJoystickDrag += HandleMoveInput;
            IngameUI.OnMainSkillClicked += HandleMainSkillClicked;
            IngameUI.OnSkillClicked += HandleSkillClicked;
        }
    }

    public void UnbindInput()
    {
        Ingame IngameUI = UIManager.Instance.GetUIByType<Ingame>();
        if (IngameUI)
        {
            IngameUI.OnJoystickDrag -= HandleMoveInput;
            IngameUI.OnSkillClicked -= HandleSkillClicked;
            IngameUI.OnMainSkillClicked -= HandleMainSkillClicked;
        }
    }

    private void HandleMainSkillClicked()
    {
        UIManager.AddDebugMessage("Main Skill Btn Clicked", LogVerbose.Warning);
    }

    private void HandleSkillClicked(int SkillIndex)
    {
        UIManager.AddDebugMessage("Skill clicked index: " + SkillIndex, LogVerbose.Warning);
    }

    public void OnStatChange(StatName Name)
    {
        if(Name == StatName.MOVE_SPD)
        {
            MoveSpeed = OwnerCharacter.GetTotalStat(Name);
        }
    }

    private void HandleMoveInput(Vector2 Input)
    {
        MoveDirection = new Vector3(Input.x, 0, Input.y);  
    }

    public void Update()
    {
        OwnerCharacter.gameObject.transform.position += MoveDirection * MoveSpeed * Time.deltaTime;
    }
}
