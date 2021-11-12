using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

/*
 * Calculating and awarding xp each time something dies.
 * XP is only awarded if at least one player participated in killing the npc.
 * XP won't be awarded if the enemy is invulnerable to damage in any part of it. This is the simplest method I could find to address the Crawltipede's inflated xp, along with any attempts to reuse that enemy by other mods.
 * The XPCap variable is there in case anyone wants to set limits on things.
 */

namespace SimpleLevels
{
    public class SimpleNPC : GlobalNPC
    {
        public double XPCap = 0.0;
        
        public override bool InstancePerEntity
        {
            get
            {
                return true;
            }
        }

        public override void NPCLoot(NPC npc)
        {
            double XP;

            XP = Math.Pow(npc.lifeMax * npc.damage * Math.Max((double)npc.defense, 1.0), 0.5);
            
            if (npc.lifeMax == 1)
                XP = 0.0;
            
            if (XPCap != 0.0)
                XP = Math.Min(XP, XPCap);

            if (npc.lastInteraction != 255 && !npc.dontTakeDamage)
            {
                if (Main.netMode == 2)
                {
                    ModPacket packet = mod.GetPacket();
                    packet.Write((byte)SimpleLevels.Message.AddXP);
                    packet.Write(XP);
                    packet.Send();
                }
                if (Main.netMode == 0)
                {
                    Main.LocalPlayer.GetModPlayer<SimplePlayer>().AddXP(XP);
                }
            }
        }
    }
}
