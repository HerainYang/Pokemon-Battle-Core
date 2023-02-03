using System.Collections.Generic;
using TalesOfRadiance.Scripts.Battle.BattleComponents;
using TalesOfRadiance.Scripts.Battle.BattleComponents.BattleLogics;
using TalesOfRadiance.Scripts.Battle.CoreClass;
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
            _heroTemplates.Add(0, new HeroTemplate(0, "海姆达尔", "TORC_ch006"));
            _heroTemplates.Add(1, new HeroTemplate(1, "神灵大祭司", "TORC_ch020"));
            _heroTemplates.Add(2, new HeroTemplate(2, "红莲哪吒", "TORC_ch028"));
            _heroTemplates.Add(3, new HeroTemplate(3, "麒麟", "TORC_ch065"));
            _heroTemplates.Add(4, new HeroTemplate(4, "女娲", "TORC_ch123"));
        }

        private void InitSkill()
        {
            _skillTemplates.Add(0, new SkillTemplate(0, "普通攻击", new []{BattleLogic.NormalAttack}));
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