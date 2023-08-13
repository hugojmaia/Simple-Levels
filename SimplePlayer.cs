using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

/*
 * This is the bulk of the mod.
 * The buffs are calculated here.
 * And so is the level up.
 */

namespace SimpleLevels
{
    class SimplePlayer : ModPlayer
    {
        public int level = 0;
        public double currentXP = 0.0;

        public float damageBuff;
        public float hpBuff;

        /*
         * Making sure everything saves and loads correctly.
         */
        
        public override void SaveData(TagCompound tag)
        {
            tag.Add("level", level);
            tag.Add("xp", currentXP);
        }

        public override void LoadData(TagCompound tag)
        {
            try
            {
                level = tag.GetInt("level");
                currentXP = tag.GetDouble("xp");
            }
            catch
            {
            }
        }
        
        public override void OnEnterWorld ()
        {
            CalculateBuffs();
        }

        /*
         * Level up and level info
         * Enforces the level cap.
         */
        
        public double GetNextLevelXP(int CurrentLevel)
        {
            if (level == ModContent.GetInstance<SimpleConfig>().LevelCap)
                return 0;
            return Math.Min((double)ModContent.GetInstance<SimpleConfig>().Level0XP * Math.Pow((1.0 + (double)ModContent.GetInstance<SimpleConfig>().XPGrowth / 100.0), (double)CurrentLevel), (double)ModContent.GetInstance<SimpleConfig>().LevelUpXPCap);
        }

        public int GetCurrentLevel()
        {
            int LevelCap = ModContent.GetInstance<SimpleConfig>().LevelCap;
            int i = level;
            while (currentXP > GetNextLevelXP(i))
            {
                currentXP -= GetNextLevelXP(i);
                i++;
            }
            if (i < LevelCap)
                return i;
            return LevelCap;
        }

        public void AddXP(double XP)
        {
            int LevelCap = ModContent.GetInstance<SimpleConfig>().LevelCap;
            currentXP += XP;
            if (level > LevelCap)
            {
                level = LevelCap;
                CalculateBuffs();
            }
            if (level == LevelCap)
                currentXP = 0;
            if (currentXP > GetNextLevelXP(level))
            {
                level = GetCurrentLevel();
                CalculateBuffs();
                Main.NewText("Level up! " + level, 63, 255, 63);
            }
        }
        
        public void ShowLevelInfo()
        {
            Main.NewText("Level: " + level, 63, 255, 63);
            Main.NewText((int)currentXP + "/" + (int)GetNextLevelXP(level) + " xp", 63, 255, 63);
            Main.NewText("Damage: +" + (damageBuff * 100) + "%", 63, 255, 63);
            Main.NewText("HP: +" + (hpBuff * 100) + "%", 63, 255, 63);
        }
        
        /*
         * Keybind
         */
        
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (SimpleLevels.ShowLevelInfoKey.JustPressed)
            {
                ShowLevelInfo();
            }
        }
        
        /*
         * Stuff that doesn't change often is calculated here.
         * This is supposed to run when the player loads in and when the player levels up.
         */
        
        public void CalculateBuffs()
        {
            damageBuff = (ModContent.GetInstance<SimpleConfig>().DamageMultiplier * level / 100f);
            hpBuff = (ModContent.GetInstance<SimpleConfig>().HPMultiplier * level / 100f);
        }
        
        /*
         * Applying damage boost
         */
        
        public void BoostIt(ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage += damageBuff;
        }

        
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            BoostIt(ref modifiers);
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            BoostIt(ref modifiers);
        }
        
        /*
         * Applying the hp buff.
         */
        
        public override void PostUpdateEquips()
        {
            Main.LocalPlayer.statLifeMax2 += (int)(Main.LocalPlayer.statLifeMax2 * hpBuff);
        }
    }
}
