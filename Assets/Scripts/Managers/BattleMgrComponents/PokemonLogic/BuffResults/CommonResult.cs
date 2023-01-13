using System.Collections.Generic;
using Enum;
using Managers.BattleMgrComponents.BattlePlayables.Skills;

namespace Managers.BattleMgrComponents.PokemonLogic.BuffResults
{
    public class CommonResult
    {
        public int Power;
        public int Damage;
        public SkillType DamageType;

        public float Accuracy;
        public bool MustHit;

        public float CriticalRate;
        public bool IsCritical;

        public CommonSkillTemplate STemplate;

        public Pokemon DebutPokemon;

        public Pokemon SkillSource;
        public Pokemon SkillTarget;

        public int Speed;

        public int Priority;
        public bool CanMove;
        public string Message;

        public bool CanBeTargeted = true;
        public bool NeedCommandFromPokemon = true;

        public int LoadSkill;
        public bool CanLoadSkill = true;
        
        public int[] PokemonStat = new int[6];

        public Weather TargetWeather;

        public int BuffKey;
        public bool CanAddBuff = true;

        public List<Pokemon> TargetsList;

        public bool ShouldContinueSkill = true;
    }
}