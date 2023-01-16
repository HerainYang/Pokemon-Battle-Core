using Managers.BattleMgrComponents.BattlePlayables.Skills;
using UnityEngine.Internal;

namespace Managers.BattleMgrComponents.PokemonLogic.BuffResults
{
    public class BeforeDamageApplyResult
    {
        public int Priority;
        public bool ShouldSuccess;
        public string Message;
        public CommonSkillTemplate Template;

        public BeforeDamageApplyResult(int priority, bool shouldSuccess, string message, CommonSkillTemplate template)
        {
            Priority = priority;
            ShouldSuccess = shouldSuccess;
            Message = message;
            Template = template;
        }

        public static BeforeDamageApplyResult Compare(BeforeDamageApplyResult self, BeforeDamageApplyResult obj)
        {
            return self.Priority > obj.Priority ? self : obj;
        }
    }
}