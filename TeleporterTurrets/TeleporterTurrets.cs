using BepInEx;
using RoR2;

namespace RuneFoxMods
{
  [BepInDependency("com.bepis.r2api")]
  //Change these
  [BepInPlugin("com.RuneFoxMods.TeleporterTurrets", "TeleporterTurrets", "1.0.0")]
  public class MyModName : BaseUnityPlugin
  {
    public void Awake()
    {
    }
  }
}