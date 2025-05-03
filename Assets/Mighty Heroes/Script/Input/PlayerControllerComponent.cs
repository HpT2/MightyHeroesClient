using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerComponent
{
    public Character OwnerCharacter;
    public Vector3 MoveDirection;
    public float MoveSpeed;

    private Transform OwnerTrans;

    public Vector3 TargetPosition;
    public Vector3 StartPosition;
    public float ElapsedTime = 0;
    public Quaternion TargetRotation;
    public bool UseExtrapolate = false;
    public Vector3 LastUpdateVelocity;
    GameObject CameraGO;
    List<Material> CameraBlockObj = new List<Material>();
    int ExtrapolateFrame;
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

        CameraGO = Camera.main.gameObject;
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
        if (OwnerCharacter.IsDeath)
        {
            return;
        }

        MoveDirection = new Vector3(Input.x, 0, Input.y);
        OwnerCharacter.Rigidbody.velocity = MoveDirection * MoveSpeed;
        LastUpdateVelocity = OwnerCharacter.Rigidbody.velocity;
    }

    public void Update()
    {
        if(OwnerCharacter.IsDeath)
        {
            return;
        }

        bool IsMoving = MoveDirection.sqrMagnitude > 0.01f;

        if (OwnerCharacter.photonView.IsMine /*|| OwnerCharacter.IsLocalControl*/)
        {
            if (MoveDirection != Vector3.zero)
            {
                OwnerCharacter.transform.rotation = Quaternion.Lerp(OwnerCharacter.transform.rotation, Quaternion.LookRotation(MoveDirection), Time.deltaTime * 5f);
            }
            
            if(!OwnerCharacter.IsAI)
            {
                ClearCameraBlockObj();
                ProcessCameraBlockObj();
            }
        }
        else
        {
            float RotateSpeed = 10 * Time.deltaTime;
            if(RotateSpeed > 0.01)
            {
                OwnerCharacter.transform.rotation = Quaternion.Lerp(OwnerCharacter.transform.rotation, TargetRotation, RotateSpeed);
            }


            if(IsMoving && (OwnerCharacter.transform.position - TargetPosition).sqrMagnitude <= 0.05f)
            {
                if (UseExtrapolate)
                {
                    ExtrapolateFrame++;
                }
                else
                {
                    UseExtrapolate = true;
                    ExtrapolateFrame = 1;
                } 
            }
            else if(!IsMoving)
            {
                UseExtrapolate = false;
            }

            if(UseExtrapolate)
            {
                if(ExtrapolateFrame <= 5)
                {
                    OwnerCharacter.Rigidbody.velocity = (MoveSpeed * MoveDirection);
                }
                else
                {
                    OwnerCharacter.Rigidbody.velocity = Vector3.zero;
                }
            }
            else if(IsMoving)
            {
                //OwnerCharacter.Rigidbody.velocity = Vector3.zero;
                //sElapsedTime += MoveSpeed * Time.deltaTime;
                OwnerCharacter.transform.position = Vector3.MoveTowards(OwnerCharacter.transform.position, TargetPosition, MoveSpeed * Time.deltaTime);
            }

        }

        if(!OwnerCharacter.IsAI)
        {
            OwnerCharacter.AnimController.SetBool("IsMoving", IsMoving);
        }
    }

    private void ClearCameraBlockObj()
    {
        foreach(Material Mat in CameraBlockObj)
        {
            Color col = Mat.color;
            col.a = 1;
            Mat.color = col;
        }
        CameraBlockObj.Clear();
    }

    private void ProcessCameraBlockObj()
    {
        Vector3 CameraPos = CameraGO.transform.position;
        Vector3 OwnerPos = OwnerCharacter.transform.position;
        Vector3 v = CameraPos - OwnerPos;
        RaycastHit Hit;
        //Debug.(OwnerPos, v, Color.red);
        if (Physics.Raycast(OwnerPos, v.normalized, out Hit, v.magnitude/*, LayerMask.NameToLayer("WorldStatic")*/))
        {
            Renderer Ren = Hit.collider.GetComponent<Renderer>();
            if (Ren)
            {
                foreach(Material Mat in Ren.materials)
                {
                    Color color = Mat.color;
                    color.a = 0.3f; // Set desired alpha
                    Mat.color = color;

                    CameraBlockObj.Add(Mat);
                }
            }
        }
    }
}
