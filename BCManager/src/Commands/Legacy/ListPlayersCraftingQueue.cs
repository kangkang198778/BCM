//using BCM.Models;
//using System.Collections.Generic;

//namespace BCM.Commands
//{
//  public class ListPlayersCraftingQueue : ListPlayers
//  {
//    public override Dictionary<string, string> jsonPlayer(PlayerInfo _pInfo)
//    {
//      Dictionary<string, string> queue = new CraftingQueue(_pInfo, _options).GetQueueItems();
//      return queue;
//    }
//    public override string displayPlayer(PlayerInfo _pInfo)
//    {
//      string output = "";
//      output += new ClientInfoList(_pInfo, _options).DisplayShort(_sep);
//      output += _sep;
//      output += new CraftingQueue(_pInfo, _options).Display(_sep);

//      return output;
//    }
//  }
//}