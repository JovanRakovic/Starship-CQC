using UnityEngine;

public class ForwardTester : MonoBehaviour
{
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, transform.forward * 5);
    }
#endif
}
