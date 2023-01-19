using System.Collections.Generic;
using Managers.BattleMgrComponents.BattlePlayables.Skills;
using Managers.BattleMgrComponents.PokemonLogic;
using PokemonLogic;
using PokemonLogic.PokemonData;

namespace Managers.BattleMgrComponents
{
    public class BattleStackItem
    {
        public Pokemon Source;
        public List<Pokemon> Targets;
        public CommonSkillTemplate Template;
        public Dictionary<Pokemon, int> Damages;
    }
}