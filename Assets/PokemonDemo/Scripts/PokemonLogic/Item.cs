using PokemonDemo.Scripts.BattlePlayables.Skills;

namespace PokemonDemo.Scripts.PokemonLogic
{
    public class Item : CommonSkillTemplate
    {
        public Item(CommonSkillTemplate template) : base(template.ID, template.Name, template.TargetType, template.ProcedureFunctions)
        {
            
        }
    }
}