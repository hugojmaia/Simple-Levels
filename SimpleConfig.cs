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
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Label("Damage")]
        [Tooltip("How much damage increases per level in %\n[0 to 1 billion]")]
        [Range(0, 1000000000)]
        [Increment(1)]
        [DrawTicks]
        [DefaultValue(2)]
        public int DamageMultiplier;

        [Label("Damage Divider")]
        [Tooltip("Divides the damage bonus by this amount, provides finer control over the bonus\n[1 to 1 billion]")]
        [Range(1, 1000000000)]
        [Increment(1)]
        [DrawTicks]
        [DefaultValue(1)]
        public int DamageDivider;

        [Label("Health")]
        [Tooltip("How much HP increases per level in %\n[0 to 1 billion]")]
        [Range(0, 1000000000)]
        [Increment(1)]
        [DrawTicks]
        [DefaultValue(1)]
        public int HPMultiplier;

        [Label("Health Divider")]
        [Tooltip("Divides the health bonus by this amount, provides finer control over the bonus\n[1 to 1 billion]")]
        [Range(1, 1000000000)]
        [Increment(1)]
        [DrawTicks]
        [DefaultValue(1)]
        public int HPDivider;

        [Label("Max level")]
        [Tooltip("The max level the player can reach\n[1 to 1 billion]")]
        [Range(1, 1000000000)]
        [Increment(1)]
        [DrawTicks]
        [DefaultValue(1000)]
        public int LevelCap;

        [Label("Level 0 xp")]
        [Tooltip("Xp required to go from level 0 to level 1.\nAll other xp requirements are based on this.\n[1 to 1 billion]")]
        [Range(1, 1000000000)]
        [Increment(100)]
        [DrawTicks]
        [DefaultValue(500)]
        public int Level0XP;

        [Label("XP Growth")]
        [Tooltip("How much more xp the next level will require in %\n[0 to 1 billion]")]
        [Range(0, 1000000000)]
        [Increment(1)]
        [DrawTicks]
        [DefaultValue(10)]
        public int XPGrowth;

        [Label("Level XP cap")]
        [Tooltip("Level ups will never require more than this amount\nSetting it to 0 will remove the cap\n[0 to 1 billion]")]
        [Range(0, 1000000000)]
        [Increment(10000000)]
        [DrawTicks]
        [DefaultValue(500000000)]
        public int LevelUpXPCap;

        [Label("Mob XP Exponent")]
        [Tooltip("Change how much xp is given on kill, 0 means xp will always be 1\nLarger values will increase xp gained\nLategame enemies will give a lot more xp than early game ones\n[0 to 2]")]
        [Range(0f, 2f)]
        [Increment(0.1f)]
        [DrawTicks]
        [DefaultValue(0.5f)]
        public float MobXPExponent;

        [Label("Only bosses give xp")]
        [DefaultValue(false)]
        public bool OnlyBossXP;

        [Label("Disable level up notification")]
        [DefaultValue(false)]
        public bool NoLevelUpNotif;
    }
}
