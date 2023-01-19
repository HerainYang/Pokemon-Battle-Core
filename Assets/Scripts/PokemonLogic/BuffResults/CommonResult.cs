using System.Collections.Generic;
using Enum;
using Managers.BattleMgrComponents.BattlePlayables.Skills;
using PokemonLogic.PokemonData;

namespace PokemonLogic.BuffResults
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
        
        public bool CanLoadSkill = true;
        public int SkillID;
        public int[] TargetsByIndices;

        public bool RunTimeSkillBaseIsItem = false;

        public Dictionary<Pokemon, List<PokemonRuntimeSkillData>> PokemonSkillsDic;

        public int[] PokemonStat = new int[6];

        public Weather TargetWeather;

        public int BuffKey;
        public bool CanAddBuff = true;

        public List<Pokemon> TargetsByPokemons;

        public bool ShouldContinueSkill = true;
    }
}