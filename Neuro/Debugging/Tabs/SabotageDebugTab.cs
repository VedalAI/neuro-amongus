using Neuro.Impostor;
using UnityEngine;

namespace Neuro.Debugging.Tabs;

[DebugTab]
public sealed class SabotageDebugTab : DebugTab
{
    public override string Name => "Sabotages";
    public override bool IsEnabled => SabotageHandler.ShouldEnqueueSabotage();

    public override void BuildUI()
    {
        if (GUILayout.Button("Sabotage O2"))
            SabotageHandler.Instance.SabotageSystem(SystemTypes.LifeSupp);
        if (GUILayout.Button("Sabotage Rector"))
            SabotageHandler.Instance.SabotageSystem(SystemTypes.Reactor);
        if (GUILayout.Button("Sabotage Electrical"))
            SabotageHandler.Instance.SabotageSystem(SystemTypes.Electrical);
        if (GUILayout.Button("Sabotage Electrical Doors"))
            SabotageHandler.Instance.SabotageDoors(SystemTypes.Electrical);
        if (GUILayout.Button("Sabotage Storage Doors"))
            SabotageHandler.Instance.SabotageDoors(SystemTypes.Storage);

        if (ShipStatus.Instance.Type == ShipStatus.MapType.Pb)
        {
            if (GUILayout.Button("Sabotage Typical Polus Combo"))
            {
                SabotageHandler.Instance.SabotageDoors(SystemTypes.Electrical);
                SabotageHandler.Instance.SabotageDoors(SystemTypes.LifeSupp);
                SabotageHandler.Instance.SabotageDoors(SystemTypes.Comms);
                SabotageHandler.Instance.SabotageDoors(SystemTypes.Weapons);
                SabotageHandler.Instance.SabotageDoors(SystemTypes.Office);
                SabotageHandler.Instance.SabotageDoors(SystemTypes.Storage);
                SabotageHandler.Instance.SabotageDoors(SystemTypes.Laboratory);
                SabotageHandler.Instance.SabotageSystem(SystemTypes.Electrical);
            }
        }
    }
}