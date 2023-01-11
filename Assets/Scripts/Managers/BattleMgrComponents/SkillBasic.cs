using System;
using Enum;
using Managers.BattleMgrComponents.PokemonLogic;

namespace Managers.BattleMgrComponents
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