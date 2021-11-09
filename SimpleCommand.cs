using System;
using Terraria;
using Terraria.ModLoader;

/*
 * I don't have the know how to put xp display on an interface yet so this is the next best thing.
 */

namespace SimpleLevels
{
    public class SimpleCommand : ModCommand
    {
        public override CommandType Type
        {
            get { return CommandType.Chat; }
        }

        public override string Command
        {
            get { return "simple"; }
        }

        public override string Usage
        {
            get { return "/simple"; }
        }

        public override string Description
        {
	        get { return "Shows your level, xp and some other things."; }
        }

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            SimplePlayer player = Main.LocalPlayer.GetModPlayer<SimplePlayer>();
            player.ShowLevelInfo();
        }
    }
}
