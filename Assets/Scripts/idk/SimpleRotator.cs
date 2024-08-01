using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRotator : MonoBehaviour
{
    private enum Axis{X, Y, Z};
    [SerializeField] private Axis axis;
    [SerializeField] private float speed = 1f;

    void Update()
    {
        Vector3 axisVec = Vector3.zero;
        switch(axis)
        {
            case Axis.X:
                axisVec = transform.right; break;
            case Axis.Y:
                axisVec = transform.up; break;
            case Axis.Z:
                axisVec = transform.forward; break;
                
        }
        transform.Rotate(axisVec, speed * Time.deltaTime * 360);
    }
}
