using System.Linq;
using BCM.Neurons;
using BCM.PersistentData;

namespace BCM.Commands
{
  public class BCEvents : BCCommandAbstract
  {
    public override void Process()
    {
      if (GameManager.Instance.World == null)
      {
        SendOutput("The world isn't loaded");

        return;
      }

      if (Params.Count == 0)
      {
        SendOutput(GetHelp());

        return;
      }

      switch (Params[0])
      {
        case "state":
        case "toggle":
        case "disable":
        case "enable":
          {
            if (Params.Count != 2)
            {
              SendOutput("Select an event neuron to view/alter the state, * for all");
              SendOutput(string.Join(",", Brain.Synapses.Select(s => s.Name).ToArray()));

              return;
            }

            if (Params[1] == "*")
            {
              foreach (var s in Brain.Synapses)
              {
                ProcessSynapseState(s);
              }

              return;
            }

            var synapse = Brain.GetSynapse(Params[1]);
            if (synapse == null)
            {
              SendOutput($"Unknown synapse {Params[1]}");

              return;
            }
            ProcessSynapseState(synapse);

            return;
          }
        case "deadisdead":
          ConfigDeadIsDead();
          return;
        case "pingkicker":
          ConfigPingKicker();
          return;
        case "tracker":
          ConfigTracker();
          return;
        case "spawnmutator":
          ConfigSpawnMutator();
          return;
        case "spawnmanager":
          ConfigSpawnManager();
          return;
        default:
          SendOutput($"Unknown neuron name {Params[0]}");
          return;
      }
    }

    private void ProcessSynapseState(Synapse synapse)
    {
      if (synapse == null)
      {
        SendOutput($"Unknown synapse {Params[1]}");

        return;
      }

      switch (Params[0])
      {
        case "state":
          break;
        case "toggle":
          synapse.IsEnabled = !synapse.IsEnabled;
          break;
        case "disable":
          synapse.IsEnabled = false;
          break;
        case "enable":
          synapse.IsEnabled = true;
          break;
      }

      SendOutput($"Synapse {synapse.Name} is {(synapse.IsEnabled ? "Enabled" : "Disabled")}");
    }

    public void ConfigSpawnMutator()
    {
      SendOutput("Under Development");
    }

    public void ConfigSpawnManager()
    {
      SendOutput("Under Development");
    }

    private void ConfigDeadIsDead()
    {
      if (Params.Count > 1)
      {
        var neuron = Brain.GetSynapseNeurons("deadisdead")?.OfType<DeadIsDead>().FirstOrDefault();
        if (neuron == null)
        {
          SendOutput("Unable to access deadisdead neuron");

          return;
        }

        switch (Params[1])
        {
          case "on":
            {
              if (Params.Count != 3)
              {
                SendOutput("deadisdead add requires a steamId");
                return;
              }
              var steamId = Params[2];
              if (steamId == "me" && SenderInfo.RemoteClientInfo != null)
              {
                steamId = SenderInfo.RemoteClientInfo.playerId;
              }
              neuron.EnableMode(steamId);
              SendOutput($"Dead is Dead enabled on player {steamId}");

              return;
            }
          case "off":
            {
              if (Params.Count != 3)
              {
                SendOutput("deadisdead off requires a steamId");
                return;
              }
              var steamId = Params[2];
              if (steamId == "me" && SenderInfo.RemoteClientInfo != null)
              {
                steamId = SenderInfo.RemoteClientInfo.playerId;
              }
              neuron.DisableMode(steamId);
              SendOutput($"Dead is Dead disabled on player {steamId}");

              return;
            }
          case "global":
            {
              bool? mode = null;
              if (Params.Count == 3 && bool.TryParse(Params[2], out var m)) mode = m;
              neuron.ToggleGlobal(mode);
              SendOutput($"Global mode set to {neuron.GlobalMode}");

              return;
            }
          case "config":
            {
              SendJson(new { Global = neuron.GlobalMode, Players = neuron.DiDModePlayers });
              return;
            }
          case "restore":
            {
              return;
            }
          case "delete":
            {
              if (Params.Count != 3)
              {
                SendOutput("deadisdead delete requires a steamId");
                return;
              }
              var steamId = Params[2];
              if (steamId == "me" && SenderInfo.RemoteClientInfo != null)
              {
                steamId = SenderInfo.RemoteClientInfo.playerId;
              }
              neuron.BackupAndDelete(steamId);

              return;
            }
          case "backup":
            {
              if (Params.Count != 3)
              {
                SendOutput("deadisdead delete requires a steamId");
                return;
              }
              var steamId = Params[2];
              if (steamId == "me" && SenderInfo.RemoteClientInfo != null)
              {
                steamId = SenderInfo.RemoteClientInfo.playerId;
              }
              neuron.BackupAndDelete(steamId, false);

              return;
            }

          default:
            SendOutput("Invalid sub command");
            return;
        }
      }

      SendOutput("DeadIsDead sub commands:");
      SendOutput("add,remove,global,config,restore,delete,backup");
    }

    public void ConfigTracker()
    {
      if (Params.Count > 1)
      {
        var neuron = Brain.GetSynapseNeurons("tracker")?.OfType<PositionTracker>().FirstOrDefault();
        if (neuron == null)
        {
          SendOutput("Unable to access tracker neuron");

          return;
        }

        switch (Params[1])
        {
          case "view":
            {
              if (Params.Count > 2)
              {
                var steamId = Params[2];
                if (steamId.Length != 17)
                {
                  SendOutput("Invalid steam id");

                  return;
                }

                var player = PersistentContainer.Instance.PlayerLogs[steamId, false];
                if (player == null)
                {
                  SendOutput($"Unable to load player information with steam id {steamId}");

                  return;
                }

                SendJson(player);
              }
              else
              {
                var keys = PersistentContainer.Instance.PlayerLogs.AllKeys().ToList();
                if (!keys.Any())
                {
                  SendOutput("No log info found");

                  return;
                }

                SendJson(keys);
              }
              return;
            }

          default:
            SendOutput("Invalid sub command");
            return;
        }
      }

      SendOutput("Tracker sub commands:");
      SendOutput("view");
    }

    public void ConfigPingKicker()
    {
      if (Params.Count > 1)
      {
        var neuron = Brain.GetSynapseNeurons("pingkicker")?.OfType<PingKicker>().FirstOrDefault();
        if (neuron == null)
        {
          SendOutput("Unable to access ping kicker neuron");

          return;
        }

        switch (Params[1])
        {
          case "add":
            {
              if (Params.Count <= 2 || Params[2].Length != 17) return;

              var steamId = Params[2];
              neuron.WhitelistPlayer(steamId);
              neuron.ClearPlayer(steamId);
              SendOutput($"SteamId {steamId} added to whitelist");
              return;
            }

          case "remove":
            {
              if (Params.Count <= 2 || Params[2].Length != 17) return;

              var steamId = Params[2];
              neuron.WhitelistPlayer(steamId, false);
              SendOutput($"SteamId {steamId} removed from whitelist");
              return;
            }

          case "limit":
            {
              if (Params.Count <= 2 || !int.TryParse(Params[2], out var limit)) return;

              neuron.SetLimit(limit);
              SendOutput($"Ping limit set to {limit}");
              return;
            }

          case "beats":
            {
              if (Params.Count <= 2 || !int.TryParse(Params[2], out var beats)) return;

              neuron.SetBeats(beats);
              SendOutput($"Beats before kick set to {beats}");
              return;
            }

          case "watchlist":
            {
              SendJson(neuron.GetWatchlist());
              return;
            }

          case "whitelist":
            {
              var neuronConfig = PersistentContainer.Instance.EventsConfig["pingkicker", false];
              if (neuronConfig != null)
              {
                SendJson(neuronConfig.Settings["Whitelist"]);
              }
              //SendJson(neuron.EventConfig.Settings["Whitelist"]);
              return;
            }

          case "config":
            {
              var neuronConfig = PersistentContainer.Instance.EventsConfig["pingkicker", false];
              if (neuronConfig != null)
              {
                SendJson(neuronConfig.Settings);
              }
              //SendJson(neuron.EventConfig.Settings);
              return;
            }

          case "clearcache":
            {
              neuron.ClearCache();
              return;
            }

          case "clearwhitelist":
            {
              neuron.ClearWhitelist();
              return;
            }

          default:
            SendOutput("Invalid sub command");
            return;
        }
      }

      SendOutput("Ping Kicker sub commands:");
      SendOutput("add, remove, limit, beats, watchlist, whitelist, config, clearcache, clearwhitelist");
    }
  }
}
