using UnityEngine;

public class PlayerControllerComponent
{
    public Character OwnerCharacter;
    public Vector3 MoveDirection;
    public float MoveSpeed;

    private Transform OwnerTrans;

    public Vector3 TargetPosition;
    public Quaternion TargetRotation;
    public bool UseExtrapolate = false;
    public PlayerControllerComponent(Character character)
    {
        OwnerCharacter = character;
    }

    public void CachedTransform(Transform transform)
    {
        OwnerTrans = transform;
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
        OwnerCharacter.Rigidbody.velocity = MoveDirection * MoveSpeed;
    }

    public void Update()
    {
        bool IsMoving = MoveDirection.sqrMagnitude > 0.01f;

        if (OwnerCharacter.photonView.IsMine || OwnerCharacter.IsLocalControl)
        {
            if (MoveDirection != Vector3.zero)
            {
                OwnerCharacter.transform.rotation = Quaternion.Lerp(OwnerCharacter.transform.rotation, Quaternion.LookRotation(MoveDirection), Time.deltaTime * 5f);
            }
        }
        else
        {
            OwnerCharacter.transform.rotation = Quaternion.Lerp(OwnerCharacter.transform.rotation, TargetRotation, Time.deltaTime * 10f);

            if(IsMoving && (OwnerCharacter.transform.position - TargetPosition).sqrMagnitude <= 0.05f)
            {
                UseExtrapolate = true;
            }
            else if(!IsMoving)
            {
                UseExtrapolate = false;
            }

            if(UseExtrapolate)
            {
                OwnerCharacter.transform.position += MoveDirection * MoveSpeed * Time.deltaTime;
            }
            else
            {
                OwnerCharacter.transform.position = Vector3.Lerp(OwnerCharacter.transform.position, TargetPosition, Time.deltaTime * MoveSpeed);
            }

        }
        
        OwnerCharacter.AnimController.SetBool("IsMoving", IsMoving);
    }
}
