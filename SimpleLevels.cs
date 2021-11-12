using Terraria;
using Terraria.ModLoader;
using System;
using System.IO;
using System.Collections.Generic;

/*
 * Main class of the mod, handling the networking stuff
 * Got a few things from kRPG's source: DataTag, Dictionary and HandlePacket
 * Trimmed them down heavily and modified some things.
 */

namespace SimpleLevels
{
    class SimpleLevels : Mod
    {
        public static ModHotKey ShowLevelInfoKey;
        public enum Message : byte {AddXP};
        public static Dictionary<Message, List<DataTag>> dataTags = new Dictionary<Message, List<DataTag>>()
        {
            { Message.AddXP, new List<DataTag>(){ DataTag.amount_double} }
        };
        
        public SimpleLevels()
        {
        }

        public override void Load()
        {
            ShowLevelInfoKey = RegisterHotKey("Show level info", "C");
        }

        public override void Unload()
        {
            ShowLevelInfoKey = null;
        }

        public class DataTag
        {
            public static DataTag amount_double = new DataTag(reader => reader.ReadDouble());
            public Func<BinaryReader, object> read;
            public DataTag(Func<BinaryReader, object> read)
            {
                this.read = read;
            }
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            Message msg = (Message)reader.ReadByte();
            Dictionary<DataTag, object> tags = new Dictionary<DataTag, object>();
            foreach (DataTag tag in dataTags[msg])
                tags.Add(tag, tag.read(reader));

            if (msg == Message.AddXP && Main.netMode == 1)
            {
                SimplePlayer character = Main.LocalPlayer.GetModPlayer<SimplePlayer>();
                character.AddXP(Convert.ToDouble(tags[DataTag.amount_double]));
            }
        }
    }
}
