using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

/*
   This code was used for testing NetworkBehaviours.
   It is te be disregarded.
*/

public class PlayerNetwork : NetworkBehaviour
{
    private Vector2 moveInput = Vector2.zero;
    [SerializeField] float moveSpeed = 3f;

    private void Start()
    {
        if(!IsOwner)
            Destroy(GetComponent<PlayerInput>());
    }
    private void Update()
    {
        if(!IsOwner) return;

        Move();
    }

    private void Move()
    {
        transform.position += new Vector3(moveInput.x, 0, moveInput.y) * (moveSpeed * Time.deltaTime);
    }

    public void OnMove(InputValue value){ moveInput = value.Get<Vector2>(); }
}
