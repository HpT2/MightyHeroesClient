using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUI : MonoBehaviour
{
    protected bool IsInited = true;

    public virtual void Init()
    {
        IsInited = true;
    }

    public virtual void Deinit()
    {
        IsInited = false;
    }

    private void OnEnable()
    {
        if (IsInited) return;
        Init();
    }

    private void OnDisable()
    {
        Deinit();
    }
}
