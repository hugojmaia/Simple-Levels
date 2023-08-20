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

        public double DamageMultiplier;
        public double DamageDivider;
        public double HPMultiplier;
        public double HPDivider;
        public int LevelCap;
        public double Level0XP;
        public double XPGrowth;
        public double LevelUpXPCap;

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
            LoadConfigs();
            CalculateBuffs();
        }

        /*
         * Level up and level info
         * Enforces the level cap.
         */
        
        public double GetNextLevelXP(int CurrentLevel)
        {
            if (level == LevelCap)
                return 0;
            if (LevelUpXPCap > 0.0)
                return Math.Min(Level0XP * Math.Pow((1.0 + (XPGrowth / 100.0)), (double)CurrentLevel), LevelUpXPCap);
            else
                return (Level0XP * Math.Pow((1.0 + (XPGrowth / 100.0)), (double)CurrentLevel));
        }

        public int GetCurrentLevel()
        {
            int i = level;
            while (currentXP > GetNextLevelXP(i))
            {
                currentXP -= GetNextLevelXP(i);
                i++;
                if (i == LevelCap)
                {
                    currentXP = 0.0;
                    return LevelCap;
                }
            }
            return i;
        }

        public void AddXP(double XP)
        {
            if (level != LevelCap)
                currentXP += XP;
            if (currentXP > GetNextLevelXP(level))
            {
                level = GetCurrentLevel();
                CalculateBuffs();
                if (!ModContent.GetInstance<SimpleConfig>().NoLevelUpNotif)
                    Main.NewText("Level up! " + level, 63, 255, 63);
            }
        }
        
        public void ShowLevelInfo()
        {
            Main.NewText("Level: " + level, 63, 255, 63);
            if (level != LevelCap)
                Main.NewText((int)currentXP + "/" + (int)GetNextLevelXP(level) + " xp", 63, 255, 63);
            else
                Main.NewText("Max level reached.", 63, 255, 63);
            Main.NewText("Damage: +" + (int)(damageBuff * 100) + "%", 63, 255, 63);
            Main.NewText("HP: +" + (int)(hpBuff * 100) + "%", 63, 255, 63);
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
         * Loading configs
         */
        
        public void LoadConfigs()
        {
            DamageMultiplier = ModContent.GetInstance<SimpleConfig>().DamageMultiplier;
            DamageDivider = ModContent.GetInstance<SimpleConfig>().DamageDivider;
            HPMultiplier = ModContent.GetInstance<SimpleConfig>().HPMultiplier;
            HPDivider = ModContent.GetInstance<SimpleConfig>().HPDivider;
            LevelCap = ModContent.GetInstance<SimpleConfig>().LevelCap;
            Level0XP = ModContent.GetInstance<SimpleConfig>().Level0XP;
            XPGrowth = ModContent.GetInstance<SimpleConfig>().XPGrowth;
            LevelUpXPCap = ModContent.GetInstance<SimpleConfig>().LevelUpXPCap;
        }

        /*
         * Stuff that doesn't change often is calculated here.
         * This is supposed to run when the player loads in and when the player levels up.
         */
        
        public void CalculateBuffs()
        {
            if (level > LevelCap)
                level = LevelCap;
            damageBuff = (float)(DamageMultiplier * level / 100.0 / DamageDivider);
            hpBuff = (float)(HPMultiplier * level / 100.0 / HPDivider);
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

        /*
         * Checking if configs have changed.
         */

        public override void PreUpdate ()
        {
            if (DamageMultiplier != (Double)ModContent.GetInstance<SimpleConfig>().DamageMultiplier ||
            DamageDivider != (Double)ModContent.GetInstance<SimpleConfig>().DamageDivider ||
            HPMultiplier != (Double)ModContent.GetInstance<SimpleConfig>().HPMultiplier ||
            HPDivider != (Double)ModContent.GetInstance<SimpleConfig>().HPDivider ||
            LevelCap != ModContent.GetInstance<SimpleConfig>().LevelCap ||
            Level0XP != (Double)ModContent.GetInstance<SimpleConfig>().Level0XP ||
            XPGrowth != (Double)ModContent.GetInstance<SimpleConfig>().XPGrowth ||
            LevelUpXPCap != (Double)ModContent.GetInstance<SimpleConfig>().LevelUpXPCap)
            {
                LoadConfigs();
                CalculateBuffs();
            }
        }
    }
}
