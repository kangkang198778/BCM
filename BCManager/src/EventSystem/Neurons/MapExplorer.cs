﻿using System;

namespace BCM.Neurons
{
  public class MapExplorer : NeuronAbstract
  {
    public MapExplorer()
    {
    }
    public override bool Fire(int b)
    {
      //Explores the map when no players are online, starting at 0,0 and making a spiral a region at a time.
      //also listens to requests for regions to be mapped, starting with each region around online players



      Log.Out(Config.ModPrefix + " MapExplorer");
      return true;
    }
  }
}
