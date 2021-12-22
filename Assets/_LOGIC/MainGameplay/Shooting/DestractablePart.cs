﻿using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DestractablePart : MonoBehaviour
{
    public Rigidbody Rigidbody => _rigidbody;
    
    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public virtual void Destroy()
    {
        _rigidbody.isKinematic = false;
        transform.parent = transform.parent.parent;
        StartCoroutine(DisablePhysiscs());
    }

    public IEnumerator DisablePhysiscs()
    {
        yield return new WaitForSeconds(10f);
        
        _rigidbody.isKinematic = true;
    }
    
}