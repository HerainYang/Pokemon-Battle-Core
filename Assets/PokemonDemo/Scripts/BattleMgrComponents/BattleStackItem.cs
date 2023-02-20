using System.Collections.Generic;
using PokemonDemo.Scripts.BattlePlayables.Skills;
using PokemonDemo.Scripts.PokemonLogic.PokemonData;

namespace PokemonDemo.Scripts.BattleMgrComponents
{
    public class BattleStackItem
    {
        public Pokemon Source;
        public List<Pokemon> Targets;
        public CommonSkillTemplate Template;
        public Dictionary<Pokemon, int> Damages;
    }
}