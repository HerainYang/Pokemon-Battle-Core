using Managers.BattleMgrComponents.BattlePlayables.Skills;

namespace PokemonLogic
{
    public class Item : CommonSkillTemplate
    {
        public Item(CommonSkillTemplate template) : base(template.ID, template.Name, template.TargetType, template.ProcedureFunctions)
        {
            
        }
    }
}