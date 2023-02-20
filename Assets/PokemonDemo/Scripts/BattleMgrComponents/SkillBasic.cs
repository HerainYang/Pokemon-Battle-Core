using System;
using PokemonDemo.Scripts.Enum;
using PokemonDemo.Scripts.PokemonLogic.PokemonData;

namespace PokemonDemo.Scripts.BattleMgrComponents
{
    public abstract class SkillBasic
    {
        protected int SkillID;
        protected int Executor;
        protected int Target;
        protected PokemonType Type;

        public abstract Tuple<SkillStatus, string> Execute(PokemonBasicInfo executor, PokemonBasicInfo target);
    }
}