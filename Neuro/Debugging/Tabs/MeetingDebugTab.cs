using System.Linq;
using Neuro.Impostor;
using UnityEngine;

namespace Neuro.Debugging.Tabs;

[DebugTab]
public sealed class MeetingDebugTab : DebugTab
{
    public override string Name => "Meeting";
    public override bool IsEnabled => MeetingHud.Instance;

    public override void BuildUI()
    {
        foreach (PlayerControl player in PlayerControl.AllPlayerControls._items.Where(p => p && p.Data is {IsDead: false, Disconnected: false}))
        {
            using (new HorizontalScope())
            {
                GUILayout.Label(player.Data.PlayerName);
                if (GUILayout.Button("Highlight")) MeetingHandler.Instance.HighlightPlayer(player.PlayerId);
                if (GUILayout.Button("Vote")) MeetingHandler.Instance.VoteForPlayer(player.PlayerId);
            }
        }

        GUILayoutUtils.HorizontalDivider();

        using (new HorizontalScope())
        {
            GUILayout.Label("Skip");
            if (GUILayout.Button("Highlight")) MeetingHandler.Instance.HighlightSkip();
            if (GUILayout.Button("Vote")) MeetingHandler.Instance.VoteForSkip();
        }
    }
}
