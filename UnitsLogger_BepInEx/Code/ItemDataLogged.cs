﻿using System.Collections.Generic;
using System.ComponentModel;
namespace UnitsLogger_BepInEx
{
    public class ItemDataLogged
    {
        public ItemDataLogged(ItemData data)
        {
            temp_rank_value = data.temp_rank_value;
            year = data.year;
            by = data.by;
            byColor = data.byColor;
            from = data.from;
            fromColor = data.fromColor;
            name = data.name;
            id = data.id;
            material = data.material;
            modifiers = data.modifiers;
            action_attack_target = data.action_attack_target;
        }

        internal int temp_rank_value;

        [DefaultValue(0)]
        public int year;

        [DefaultValue("")]
        public string by = string.Empty;

        internal string byColor = string.Empty;

        [DefaultValue("")]
        public string from = string.Empty;

        internal string fromColor = string.Empty;

        [DefaultValue("")]
        public string name = string.Empty;

        [DefaultValue(0)]
        public int kills;

        public string id;

        public string material;

        public List<string> modifiers = new List<string>();

        public AttackAction action_attack_target;
    }
}
