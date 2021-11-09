using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.GameInput;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

/*
 * Comments changed on the middle of the code.
 */

namespace SimpleLevels
{
    class SimplePlayer : ModPlayer
    {
        public int level = 0;
        public double totalXP = 0.0;
        
        public double Level0XP = 500.0;
        public double XPGrowth = 1.1;
        public double LevelUpXPCap = 200000000.0;
        public double DamageBoostPerLevel = 0.02;
        public double DamageReductionPerLevel = 0.02;
        public double DamageReductionCap = 0.25;
        public double ManaReductionPerLevel = 0.02;
        public double ManaReductionCap = 0.25;
        public double HPPerLevel = 0.01;
        public double HPRegenPerLevel = 0.04;
        public double RespawnTimerReductionPerLevel = 0.0075;
        public double RespawnTimerReductionCap = 0.75;
        
        public int WingTimeBoostPerLevel = 6;
        public int WingTimeBoostStartingLevel = 50;
        public int WingTimeBoostCap = 300;
        public double MaxRunSpeedIncreasePerLevel = 0.01;
        public double MaxRunSpeedIncreaseCap = 0.5;
        public double RunAccelerationIncreasePerLevel = 0.025;
        public double RunAccelerationIncreaseCap = 1.25;
        public double ItemUseTimeSpeedupPerLevel = 0.005;
        public double ItemUseTimeSpeedupCap = 0.50;
        
        public int hpIncrease;
        public int lifeRegenIncrease;
        public int wingTimeBoost;
        public double damageTakenMultiplier;
        public float manaCostMultiplier;
        public float maxRunSpeedMultiplier;
        public float runAccelerationMultiplier;
        public double respawnTimerReduction;
        public float itemUseTimeSpeedup;

		/*
		 * Making sure everything saves and loads correctly.
		 */
		
        public override TagCompound Save()
        {
            return new TagCompound
            {
                {"level", level},
                {"xp", totalXP}
            };
        }

        public override void Load(TagCompound tag)
        {
            try
            {
                level = tag.GetInt("level");
                totalXP = tag.GetDouble("xp");
            }
            catch (SystemException e)
            {
                ErrorLogger.Log("@Level&XP :: " + e.ToString());
            }
        }
        
        public override void OnEnterWorld (Player player)
        {
            CalculateBuffs();
        }

		/*
		 * Level up and level info
		 */
		
        public double GetNextLevelXP(int CurrentLevel)
        {
            return Math.Min(Level0XP * Math.Pow(XPGrowth, (double)CurrentLevel), LevelUpXPCap);
        }

        public int GetCurrentLevel()
        {
            int i = level;
            while (totalXP > GetNextLevelXP(i))
            {
                totalXP -= GetNextLevelXP(i);
                i++;
            }
            return i;
        }

        public void AddXP(double XP)
        {
            totalXP += XP;
            if (totalXP > GetNextLevelXP(level))
            {
                level = GetCurrentLevel();
                CalculateBuffs();
                Main.NewText("You're now level " + level + "!", 63, 255, 63);
            }
        }
        
        public void SetLevel(int newLevel)
        {
            level = newLevel;
            totalXP = 0.0;
            CalculateBuffs();
        }
        
        public void ShowLevelInfo()
        {
            Main.NewText("Your level is " + level + ".", 63, 255, 63);
            Main.NewText("You need " + (int)(GetNextLevelXP(level) - totalXP) + " xp to level up.", 63, 255, 63);
            Main.NewText("Your level gives you a " + (level * DamageBoostPerLevel * 100) + "% bonus to damage.", 63, 255, 63);
            Main.NewText("Plus " + hpIncrease + " HP and " + Math.Round((int)(HPRegenPerLevel * level / 2.0) * 0.5, 1) + " hp per second", 63, 255, 63);
            Main.NewText("Mana cost cut by " + (int)((1.0f - manaCostMultiplier)*100) + "%.", 63, 255, 63);
            Main.NewText("Damage taken cut by " + (int)((1.0 - damageTakenMultiplier)*100) + "%.", 63, 255, 63);
            Main.NewText("Item use speed increased by " + ((int)(itemUseTimeSpeedup * 100) - 100) + "%.", 63, 255, 63);
			Main.NewText("Respawn time cut by " + (int)(respawnTimerReduction * 100) + "%.", 63, 255, 63);
            Main.NewText("Move speed increased by " + ((int)(maxRunSpeedMultiplier * 100) - 100) + "%.", 63, 255, 63);
            
            if (wingTimeBoost > 0)
            {
                Main.NewText("Wing time increased by " + Math.Round(wingTimeBoost/60.0, 1) + " seconds.", 63, 255, 63);
            }
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
		 * Applying damage boost
		 */
		
        public void BoostIt(ref int damage)
        {
            damage = (int)(damage * (1.0 + DamageBoostPerLevel * level));
        }

        
        public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            BoostIt(ref damage);
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            BoostIt(ref damage);
        }
        
        public override void ModifyHitPvp(Item item, Player target, ref int damage, ref bool crit)
        {
            BoostIt(ref damage);
        }

        public override void ModifyHitPvpWithProj(Projectile proj, Player target, ref int damage, ref bool crit)
        {
            BoostIt(ref damage);
        }
        
		/*
		 * Applying damage reduction
		 */
		
        public void ReduceIt(ref int damage)
        {
            int reduction = Main.LocalPlayer.statDefense;
            
            if (Main.expertMode)
                reduction = (int)(reduction * 0.75);
            else
                reduction = (int)(reduction * 0.5);
            
            if (damage - reduction > 0)
            {
                damage -= reduction;
                damage = (int)(damage * damageTakenMultiplier);
                damage += reduction;
            }
        }
        
        public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
        {
            ReduceIt (ref damage);
        }
        
        public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
        {
            ReduceIt (ref damage);
        }

        /*
         * For performance reasons, removed all calculations that were done every frame and moved them here.
         * This is supposed to run when the player loads in and when the player levels up.
         * Probably broke a lot of things in doing this.
         */
        
        public void CalculateBuffs()
        {
            hpIncrease = (int)(Main.LocalPlayer.statLifeMax * HPPerLevel * level);
            lifeRegenIncrease = (int)(HPRegenPerLevel * level);
            damageTakenMultiplier = Math.Max(1.0/(1.0 + DamageReductionPerLevel * level), DamageReductionCap);
            manaCostMultiplier = (float)Math.Max(1.0/(1.0 + ManaReductionPerLevel * level), ManaReductionCap);
            respawnTimerReduction = Math.Min(level * RespawnTimerReductionPerLevel, RespawnTimerReductionCap);
            itemUseTimeSpeedup = (float)(1.0 + Math.Min(level * ItemUseTimeSpeedupPerLevel, ItemUseTimeSpeedupCap));
            maxRunSpeedMultiplier = (float)(Math.Min(level * MaxRunSpeedIncreasePerLevel, MaxRunSpeedIncreaseCap) + 1.0);
            runAccelerationMultiplier = (float)(Math.Min(level * RunAccelerationIncreasePerLevel, RunAccelerationIncreaseCap) + 1.0);
            wingTimeBoost = Math.Max(Math.Min(WingTimeBoostPerLevel * (level - WingTimeBoostStartingLevel), WingTimeBoostCap), 0);
        }
        
		/*
		 * Applying the remaining buffs.
		 */
		
        public override void PreUpdateBuffs()
        {
            Main.LocalPlayer.statLifeMax2 += hpIncrease;
            Main.LocalPlayer.lifeRegen += lifeRegenIncrease;
        }
        
        public override void PostUpdateEquips()
        {
            Main.LocalPlayer.manaCost *= manaCostMultiplier;
            Main.LocalPlayer.maxRunSpeed = Main.LocalPlayer.maxRunSpeed * maxRunSpeedMultiplier;
            Main.LocalPlayer.runAcceleration = Main.LocalPlayer.runAcceleration * runAccelerationMultiplier;
            Main.LocalPlayer.wingTimeMax += wingTimeBoost;
        }
        
        public override float UseTimeMultiplier (Item item)
        {
            if (item.useTime < 3)
                return 1.0f;
            return itemUseTimeSpeedup;
        }
        
        public override void Kill (double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            Main.LocalPlayer.respawnTimer -= (int)(Main.LocalPlayer.respawnTimer * respawnTimerReduction);
        }
    }
}
