using System;
using Terraria;
using Terraria.ModLoader;

/*
 * Yay for copy pasting code.
 */

namespace SimpleLevels
{
    public class SimpleSetLevelCommand : ModCommand
    {
        public override CommandType Type
        {
            get { return CommandType.Chat; }
        }

        public override string Command
        {
            get { return "simpleSetLevel"; }
        }

        public override string Usage
        {
            get { return "/simpleSetLevel newLevel"; }
        }

        public override string Description
        {
	        get { return "Sets your level to the given number and wipes current xp."; }
        }

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            SimplePlayer player = Main.LocalPlayer.GetModPlayer<SimplePlayer>();
            int newLevel = int.Parse(args[0]);
            
            player.SetLevel(newLevel);
            
            player.ShowLevelInfo();
        }
    }
}
