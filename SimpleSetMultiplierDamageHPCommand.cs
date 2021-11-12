using System;
using Terraria;
using Terraria.ModLoader;

/*
 * Allows the player to manually set 
 */

namespace SimpleLevels
{
    public class SimpleSetMultiplierDamageHPCommand : ModCommand
    {
        public override CommandType Type
        {
            get { return CommandType.Chat; }
        }

        public override string Command
        {
            get { return "simpleSetMultiplierDamageHP"; }
        }

        public override string Usage
        {
            get { return "/simpleSetMultiplierDamageHP multDamage multHP"; }
        }

        public override string Description
        {
            get { return "Sets the multiplier for the damage and hp buffs, 0.1% steps, caps at 1000."; }
        }

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            SimplePlayer player = Main.LocalPlayer.GetModPlayer<SimplePlayer>();
            
            int damage = int.Parse(args[0]);
            int hp = int.Parse(args[1]);
            
            player.SetMultiplierDamageHP(damage, hp);
        }
    }
}