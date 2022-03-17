using BepInEx;
using RoR2;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;


namespace RuneFoxMods
{

  //Change these
  [BepInPlugin("com.RuneFoxMods.TeleporterTurrets", "TeleporterTurrets", "1.0.3")]
  public class TeleporterTurrets : BaseUnityPlugin
  {
    public static String[] TeleportableTurrets = new string[]
    {
      "Turret1"
    };

    public void Awake()
    {
      //TeleporterInteraction - onTeleporterBeginChargingGlobal
      //This looks like the function to hook into to move all active turrets to teleporter
      //try gameobject/self from this class to get position of teleporter
      On.RoR2.TeleporterInteraction.OnInteractionBegin += TeleporterInteraction_OnInteractionBegin;
    }

    private void TeleporterInteraction_OnInteractionBegin(On.RoR2.TeleporterInteraction.orig_OnInteractionBegin orig, TeleporterInteraction self, Interactor activator)
    {
        //if (!self.isIdleToCharging)

        //Chat.AddMessage("Mod says teleporter has been activated");
        //if(self.mainStateMachine)
        //  Chat.AddMessage(self.mainStateMachine.state.ToString());
        //if(self.mainStateMachine.state != EntityStates.LunarTeleporter.Idle)
        if(!self.isIdle)
        {
          //Chat.AddMessage("teleporter not idle");
          orig(self, activator);
          return;
        }

        var masters = CharacterMaster.readOnlyInstancesList;
        List<CharacterMaster> masterList = masters.Cast<CharacterMaster>().ToList();
        List<CharacterMaster> TurretList = new List<CharacterMaster>();

        foreach (CharacterMaster cm in masterList)
        {
          if (cm == null)
            continue;

          //Chat.AddMessage(cm.name);
          foreach (var turretName in TeleportableTurrets)
          {
            if (cm.name.StartsWith(turretName))
            {
              //Chat.AddMessage("Teleportable Turret is here");
              TurretList.Add(cm);
              //Chat.AddMessage(self.name); 
            }
          }
        }

        int i = 0;
        int count = TurretList.Count;
      //Chat.AddMessage("Turret Count: " + count);
      //Chat.AddMessage("Teleporter Pos: " + self.transform.position);

      //teleport turrets
      foreach (var turret in TurretList)
        {
          // 7.5 is the magic number to have all turrets on the teleporter platform
          // needs to be slightly larger for the primordial telepot
          float Radius = 7f;
          float radianInc = Mathf.Deg2Rad * 360f / count;
          Vector3 point1 = new Vector3(Mathf.Cos(radianInc * i) * Radius, 0.25f, Mathf.Sin(radianInc * i) * Radius);

          //float x = self.transform.position.x + Mathf.Sin(3.14f * i / count) * Radius;
          //float y = self.transform.position.y + 0.9f;//keep the turrets from sinking into the teleporter
          //float z = self.transform.position.z + Mathf.Cos(3.14f * i / count) * Radius;
          //Vector3 position = new Vector3(x, y, z);

            
          i++;

          var targetFootPos = self.transform.position + point1;
          var turretBody = turret.GetBody();

          //I just copied the TeleportBody code and removed the new if(body) section that was added to fix the inertia glitch as that was breaking my code since turrets don't have characterMotors
          Vector3 b = new Vector3(0f, 0.1f, 0f);
          Vector3 b2 = turretBody.footPosition - turretBody.transform.position;
          //Chat.AddMessage("teleporting turret " + i + " at position " + point1);
          TeleportHelper.TeleportGameObject(turretBody.gameObject, targetFootPos - b2 + b);

          //Chat.AddMessage("HasAuth: " + Util.HasEffectiveAuthority(turretBody.gameObject));
          //TeleportHelper.TeleportBody(turretBody, self.transform.position + point1);


        }

        orig(self, activator);
    }
}}
