// I dug this out from one of your only 3 commits in this repo. YW

/*
    public void ReportFindings()
    {
        foreach ((PlayerControl player, LastSeenPlayer lastSeen) in _playerLocations)
        {
            if (!player || player.AmOwner) continue;

            if (lastSeen.location == "") continue;
            if (lastSeen.dead)
            {
                Info(player.name + " was found dead in " + lastSeen.location + " " + Mathf.Round(Time.timeSinceLevelLoad - lastSeen.time) + " seconds ago.");
                Info("Witnesses:");
                foreach (PlayerControl witness in lastSeen.witnesses) Info(witness.name);
            }
            else
            {
                Info(player.name + " was last seen in " + lastSeen.location + " " + Mathf.Round(Time.timeSinceLevelLoad - lastSeen.time) + " seconds ago.");

                // Report if we saw the player vent right in front of us
                if (lastSeen.sawVent)
                    Info("I saw " + player.name + " vent right in front of me!");

                // Determine how much time the player was visible to Neuro-sama for
                float gamePercentage = lastSeen.gameTimeVisible / Time.timeSinceLevelLoad;
                float roundPercentage = lastSeen.roundTimeVisible / (Time.timeSinceLevelLoad - roundStartTime);
                TimeSpan gameTime = new(0, 0, (int) Math.Floor(lastSeen.gameTimeVisible));
                TimeSpan roundTime = new(0, 0, (int) Math.Floor(lastSeen.roundTimeVisible));
                Info($"{player.name} has spent {gameTime.Minutes} minutes and {gameTime.Seconds} seconds near me this game ({gamePercentage * 100.0f:0.0}% of the game)");
                Info($"{player.name} has spent {roundTime.Minutes} minutes and {roundTime.Seconds} seconds near me this round ({roundPercentage * 100.0f:0.0}% of the round)");
            }
        }
    }
*/
