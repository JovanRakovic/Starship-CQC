using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class PlayerHandler : NetworkBehaviour
{
   public delegate void GetName(PlayerHandler handler);
   public static GetName getName;
   NetworkVariable<FixedString32Bytes> username = new NetworkVariable<FixedString32Bytes>("Temp", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

   private void Awake()
   {
      Object.DontDestroyOnLoad(gameObject);
   }

   public override void OnNetworkSpawn()
   {
      base.OnNetworkSpawn();
      if(IsOwner)
         getName?.Invoke(this);
   }

   public void SetUsername(string un)
   {
      username.Value = un;
   }

   public string GetUsername()
   {
      return username.Value.ToString();
   }
}
