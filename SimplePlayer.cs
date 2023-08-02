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
        public int damageMultiplier = 20;
        public int hpMultiplier = 10;
        
        public float damageBuff = 0.0f;
        public float hpBuff = 0.0f;
        
        public double Level0XP = 500.0;
        public int LevelCap = 1000;
        public double XPGrowth = 1.1;
        public double LevelUpXPCap = 500000000.0;
        public double DamageStep = 0.001;
        public double HPStep = 0.001;
        public int DamageMultiplierCap = 1000;
        public int HPMultiplierCap = 1000;

        /*
         * Making sure everything saves and loads correctly.
         */
        
        public override void SaveData(TagCompound tag)
        {
            tag.Add("level", level);
            tag.Add("xp", currentXP);
            tag.Add("damage", damageMultiplier);
            tag.Add("hp", hpMultiplier);
        }

        public override void LoadData(TagCompound tag)
        {
            try
            {
                level = tag.GetInt("level");
                currentXP = tag.GetDouble("xp");
                damageMultiplier = tag.GetInt("damage");
                hpMultiplier = tag.GetInt("hp");
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
         * Added precautions in case the level ends up above the cap.
         */
        
        public double GetNextLevelXP(int CurrentLevel)
        {
            if (level == LevelCap)
                return 0;
            return Math.Min(Level0XP * Math.Pow(XPGrowth, (double)CurrentLevel), LevelUpXPCap);
        }

        public int GetCurrentLevel()
        {
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
         * Change the multiplier for hp and damage
         */
        
        public void SetMultiplierDamageHP(int damage, int hp)
        {
            if (damage >= 0 && damage <= DamageMultiplierCap)
                damageMultiplier = damage;
            Main.NewText("Damage Multiplier set to " + damageMultiplier, 63, 255, 63);
            if (hp >= 0 && hp <= HPMultiplierCap)
                hpMultiplier = hp;
            Main.NewText("HP Multiplier set to " + hpMultiplier, 63, 255, 63);
            CalculateBuffs();
        }

        /*
         * Stuff that doesn't change often is calculated here.
         * This is supposed to run when the player loads in and when the player levels up.
         * Added a sanity check in case the multipliers end up outside of the cap.
         */
        
        public void CalculateBuffs()
        {
            if (damageMultiplier > DamageMultiplierCap)
                damageMultiplier = DamageMultiplierCap;
            if (damageMultiplier < 0)
                damageMultiplier = 0;
            damageBuff = (float)(damageMultiplier * DamageStep * level);
            if (hpMultiplier > HPMultiplierCap)
                hpMultiplier = HPMultiplierCap;
            if (hpMultiplier < 0)
                hpMultiplier = 0;
            hpBuff = (float)(hpMultiplier * HPStep * level);
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
