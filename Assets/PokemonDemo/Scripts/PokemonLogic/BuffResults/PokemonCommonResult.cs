using System;
using System.Collections.Generic;
using CoreScripts.BattleComponents;
using PokemonDemo.Scripts.BattlePlayables.Skills;
using PokemonDemo.Scripts.Enum;
using PokemonDemo.Scripts.PokemonLogic.PokemonData;

namespace PokemonDemo.Scripts.PokemonLogic.BuffResults
{
    public class PokemonCommonResult : ASkillResult
    {
        public int Power;
        public int Damage;
        
        public bool ShouldSuccess;

        public float Accuracy;
        public bool MustHit;

        public float CriticalRate;
        public bool IsCritical;

        public CommonSkillTemplate STemplate;

        public Pokemon DebutPokemon;

        public Pokemon SkillSource;
        public Pokemon SkillTarget;

        public int Priority;
        public bool CanMove;
        public string Message;

        public bool CanBeTargeted = true;
        
        public bool CanLoadSkill = true;
        public int[] TargetsByIndices;

        public bool RunTimeSkillBaseIsItem = false;

        public Dictionary<Pokemon, List<PokemonRuntimeSkillData>> PokemonSkillsDic;

        public int[] PokemonStat = new int[6];

        public Weather TargetWeather;

        public int BuffKey;
        public bool CanAddBuff = true;

        public List<Pokemon> TargetsByPokemons;

        public bool ShouldContinueSkill = true;

        public PokemonCommonResult Copy()
        {
            PokemonCommonResult output = new PokemonCommonResult()
            {
                Power = Power,
                Damage = Damage,
                ShouldSuccess = ShouldSuccess,
                Accuracy = Accuracy,
                MustHit = MustHit,
                CriticalRate = CriticalRate,
                IsCritical = IsCritical,
                
                DebutPokemon = DebutPokemon,
                SkillSource = SkillSource,
                SkillTarget = SkillTarget,
                
                Priority = Priority,
                CanMove = CanMove,
                Message = Message,
                
                CanBeTargeted = CanBeTargeted,
                CanLoadSkill = CanLoadSkill,
                TargetsByIndices = new int[TargetsByIndices.Length],
                RunTimeSkillBaseIsItem = RunTimeSkillBaseIsItem,
                PokemonSkillsDic = new Dictionary<Pokemon, List<PokemonRuntimeSkillData>>(),
                PokemonStat = new int[6],
                TargetWeather = TargetWeather,
                BuffKey = BuffKey,
                CanAddBuff = CanAddBuff,
                TargetsByPokemons = TargetsByPokemons,
                ShouldContinueSkill = ShouldContinueSkill
            };
            output.STemplate = CommonSkillTemplate.CopySkill(STemplate);
            Array.Copy(TargetsByIndices, output.TargetsByIndices, TargetsByIndices.Length);
            Array.Copy(PokemonStat, output.PokemonStat, PokemonStat.Length);
            return output;
        }
    }
}