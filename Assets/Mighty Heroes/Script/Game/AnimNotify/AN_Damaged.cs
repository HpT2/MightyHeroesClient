using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AN_Damaged : StateMachineBehaviour
{
    public Material DamagedLayerMat;
    public string LayerObjName;
    private int AddedMatIndex;
    public float EndDamagedTime;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        AddedMatIndex = -1;
        GameObject Owner = animator.gameObject; 
        if(Owner)
        {
            GameObject LayerObj = Owner.transform.Find(LayerObjName)?.transform?.gameObject;
            if(LayerObj)
            {
                SkinnedMeshRenderer MeshRenderer = LayerObj.GetComponent<SkinnedMeshRenderer>();
                if(MeshRenderer)
                {
                    Material[] currentMaterials = MeshRenderer.materials;
                    Material[] updatedMaterials = new Material[currentMaterials.Length + 1];

                    for (int i = 0; i < currentMaterials.Length; i++)
                    {
                        updatedMaterials[i] = currentMaterials[i];
                    }

                    AddedMatIndex = updatedMaterials.Length - 1;
                    updatedMaterials[AddedMatIndex] = Material.Instantiate(DamagedLayerMat);
                    MeshRenderer.materials = updatedMaterials;
                }
            }
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        if(stateInfo.normalizedTime >= EndDamagedTime && AddedMatIndex != -1)
        {
            GameObject Owner = animator.gameObject;
            if (Owner)
            {
                GameObject LayerObj = Owner.transform.Find(LayerObjName)?.transform?.gameObject;
                if (LayerObj)
                {
                    SkinnedMeshRenderer MeshRenderer = LayerObj.GetComponent<SkinnedMeshRenderer>();
                    if (MeshRenderer)
                    {
                        List<Material> Mats = new List<Material>(MeshRenderer.materials);
                        Mats.RemoveAt(AddedMatIndex);
                        MeshRenderer.materials = Mats.ToArray();
                        AddedMatIndex = -1;
                    }
                }
            }
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (AddedMatIndex != -1)
        {
            GameObject Owner = animator.gameObject;
            if (Owner)
            {
                GameObject LayerObj = Owner.transform.Find(LayerObjName)?.transform?.gameObject;
                if (LayerObj)
                {
                    SkinnedMeshRenderer MeshRenderer = LayerObj.GetComponent<SkinnedMeshRenderer>();
                    if (MeshRenderer)
                    {
                        List<Material> Mats = new List<Material>(MeshRenderer.materials);
                        Mats.RemoveAt(AddedMatIndex);
                        MeshRenderer.materials = Mats.ToArray();
                        AddedMatIndex = -1;
                    }
                }
            }
        }
    }
}