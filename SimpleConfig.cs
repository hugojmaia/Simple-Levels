using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;

/*
 * Options, so I never have to make people touch that janky command ever again.
 */

namespace SimpleLevels
{
    public class SimpleConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Label("Damage")]
        [Tooltip("How much damage increases per level in %")]
        [Range(0f, 100f)]
        [Increment(1f)]
        [DrawTicks]
        [DefaultValue(2f)]
        public float DamageMultiplier;

        [Label("Health")]
        [Tooltip("How much HP increases per level in %")]
        [Range(0f, 100f)]
        [Increment(1f)]
        [DrawTicks]
        [DefaultValue(1f)]
        public float HPMultiplier;

        [Label("Max level")]
        [Tooltip("The max level the player can reach")]
        [Range(10, 10000)]
        [Increment(1)]
        [DrawTicks]
        [DefaultValue(1000)]
        public int LevelCap;

        [Label("Level 0 xp")]
        [Tooltip("Xp required to go from level 0 to level 1. All other xp requirements are based on this")]
        [Range(100f, 10000f)]
        [Increment(100f)]
        [DrawTicks]
        [DefaultValue(500f)]
        public float Level0XP;

        [Label("XP Growth")]
        [Tooltip("How much more xp the next level will require in %")]
        [Range(0f, 100f)]
        [Increment(1f)]
        [DrawTicks]
        [DefaultValue(10f)]
        public float XPGrowth;

        [Label("Level XP cap")]
        [Tooltip("Level ups will never require more than this amount")]
        [Range(10000000f, 1000000000f)]
        [Increment(10000000f)]
        [DrawTicks]
        [DefaultValue(500000000f)]
        public float LevelUpXPCap;

        [Label("Mob XP Exponent")]
        [Tooltip("Change how much xp is given on kill, 0 means xp will always be 1, larger values will increase xp gained, lategame enemies will give a lot more xp than early game ones")]
        [Range(0f, 2f)]
        [Increment(0.1f)]
        [DrawTicks]
        [DefaultValue(0.5f)]
        public float MobXPExponent;

        public override void OnChanged()
        {
            try
            {
                Main.LocalPlayer.GetModPlayer<SimplePlayer>().CalculateBuffs();
            }
            catch
            {
            }
        }
    }
}