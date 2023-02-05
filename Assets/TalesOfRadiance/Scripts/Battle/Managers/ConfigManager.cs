using System.Collections.Generic;
using TalesOfRadiance.Scripts.Battle.BattleComponents;
using TalesOfRadiance.Scripts.Battle.BattleComponents.BattleLogics;
using UnityEngine;

namespace TalesOfRadiance.Scripts.Battle.Managers
{
    public class ConfigManager
    {
        private static ConfigManager _instance;
        private readonly Dictionary<int, HeroTemplate> _heroTemplates = new Dictionary<int, HeroTemplate>();
        private readonly Dictionary<int, SkillTemplate> _skillTemplates = new Dictionary<int, SkillTemplate>();

        public static ConfigManager Instance
        {
            get { return _instance ??= new ConfigManager(); }
        }

        private ConfigManager()
        {
            InitHero();
            InitSkill();
        }

        private readonly int[][][] _squadTypeConfig = new[]
        {
            new[]
            {
                new[] { 1, 1, 0 },
                new[] { 0, 0, 1 },
                new[] { 1, 1, 0 }
            },
            new[]
            {
                new[] { 0, 1, 1 },
                new[] { 1, 0, 0 },
                new[] { 0, 1, 1 }
            },
            new[]
            {
                new[] { 0, 0, 1 },
                new[] { 1, 1, 1 },
                new[] { 0, 0, 1 }
            },
            new[]
            {
                new[] { 0, 0, 1 },
                new[] { 1, 1, 1 },
                new[] { 0, 0, 1 }
            },
            new[]
            {
                new[] { 0, 0, 1 },
                new[] { 1, 1, 1 },
                new[] { 0, 0, 1 }
            },
            new[]
            {
                new[] { 0, 0, 1 },
                new[] { 1, 1, 1 },
                new[] { 0, 0, 1 }
            },
        };

        private readonly Vector3[] _anchorPosition = new[]
        {
            new Vector3(-2, 0, 2),
            new Vector3(0, 0, 2),
            new Vector3(2, 0, 2),

            new Vector3(-2, 0, 0),
            new Vector3(0, 0, 0),
            new Vector3(2, 0, 0),

            new Vector3(-2, 0, -2),
            new Vector3(0, 0, -2),
            new Vector3(2, 0, -2),
        };

        private void InitHero()
        {
            _heroTemplates.Add(0, new HeroTemplate(0, "海姆达尔", "TORC_ch006")
            {
                Attack = 75396,
                MaxHealth = 1052726,
                Defence = 2542,
                Speed = 1804,

                CriticalRate = 0.02f,
                CriticalDamage = 1.64f,
                ControlRate = 0.125f,
                AntiControl = 0.02f,
                AntiCritical = 0.325f,
                Accuracy = 1.02f,
                DamageAvoid = 0.1f,
                DodgeRate = 0.02f,
                HealRate = 0f,
                GetHealRate = 0f,
                DamageIncrease = 0.021f,
                PhysicalDamageIncrease = 0f,
                SpecialDamageIncrease = 0f,
                PhysicalDamageAvoid = 0.105f,
                SpecialDamageAvoid = 0f,
                SustainDamageIncrease = 0f,
                SustainDamageAvoid = 0f,
            });
            _heroTemplates.Add(1, new HeroTemplate(1, "神灵大祭司", "TORC_ch020")
            {
                Attack = 82849,
                MaxHealth = 820257,
                Defence = 2157,
                Speed = 1817,

                CriticalRate = 0.02f,
                CriticalDamage = 1.54f,
                ControlRate = 0.02f,
                AntiControl = 0.125f,
                AntiCritical = 0.125f,
                Accuracy = 1.02f,
                DamageAvoid = 0f,
                DodgeRate = 0.02f,
                HealRate = 0.1f,
                GetHealRate = 0f,
                DamageIncrease = 0.021f,
                PhysicalDamageIncrease = 0.1f,
                SpecialDamageIncrease = 0f,
                PhysicalDamageAvoid = 0f,
                SpecialDamageAvoid = 0.15f,
                SustainDamageIncrease = 0f,
                SustainDamageAvoid = 0.003f,
            });
            _heroTemplates.Add(2, new HeroTemplate(2, "红莲哪吒", "TORC_ch028")
            {
                Attack = 2036880,
                MaxHealth = 1561792,
                Defence = 3480,
                Speed = 2007,

                CriticalRate = 0.495f,
                CriticalDamage = 1.95f,
                ControlRate = 0.02f,
                AntiControl = 0.02f,
                AntiCritical = 0.25f,
                Accuracy = 1.02f,
                DamageAvoid = 0f,
                DodgeRate = 0.02f,
                HealRate = 0f,
                GetHealRate = 0f,
                DamageIncrease = 0.171f,
                PhysicalDamageIncrease = 0f,
                SpecialDamageIncrease = 0f,
                PhysicalDamageAvoid = 0f,
                SpecialDamageAvoid = 0f,
                SustainDamageIncrease = 0f,
                SustainDamageAvoid = 0.003f,
            });
            _heroTemplates.Add(3, new HeroTemplate(3, "麒麟", "TORC_ch065")
            {
                Attack = 109362,
                MaxHealth = 724466,
                Defence = 2739,
                Speed = 1829,

                CriticalRate = 0.285f,
                CriticalDamage = 1.85f,
                ControlRate = 0.02f,
                AntiControl = 0.02f,
                AntiCritical = 0.07f,
                Accuracy = 1.02f,
                DamageAvoid = 0f,
                DodgeRate = 0.02f,
                HealRate = 0f,
                GetHealRate = 0f,
                DamageIncrease = 0.171f,
                PhysicalDamageIncrease = 0.15f,
                SpecialDamageIncrease = 0f,
                PhysicalDamageAvoid = 0.15f,
                SpecialDamageAvoid = 0f,
                SustainDamageIncrease = 0f,
                SustainDamageAvoid = 0.003f,
            });
            _heroTemplates.Add(4, new HeroTemplate(4, "女娲", "TORC_ch123")
            {
                Attack = 103524,
                MaxHealth = 1027987,
                Defence = 2265,
                Speed = 2224,

                CriticalRate = 0.02f,
                CriticalDamage = 1.54f,
                ControlRate = 0.02f,
                AntiControl = 0.325f,
                AntiCritical = 0.175f,
                Accuracy = 1.02f,
                DamageAvoid = 0f,
                DodgeRate = 0.02f,
                HealRate = 0f,
                GetHealRate = 0f,
                DamageIncrease = 0.021f,
                PhysicalDamageIncrease = 0f,
                SpecialDamageIncrease = 0f,
                PhysicalDamageAvoid = 0f,
                SpecialDamageAvoid = 0.105f,
                SustainDamageIncrease = 0f,
                SustainDamageAvoid = 0.093f,
            });
        }

        private void InitSkill()
        {
            _skillTemplates.Add(0, new SkillTemplate(0, "普通攻击", new[] { BattleLogic.NormalAttack })
            {
                DamageIncreaseRate = 1f
            });
        }

        public HeroTemplate GetHeroTemplateByID(int id)
        {
            return _heroTemplates[id];
        }

        public int[][] GetSquadTypeByID(int id)
        {
            return _squadTypeConfig[id];
        }

        public Vector3 GetAnchorPosition(int id)
        {
            return _anchorPosition[id];
        }

        public SkillTemplate GetSkillTemplateByID(int id)
        {
            return _skillTemplates[id];
        }
    }
}