using TalesOfRadiance.Scripts.Battle.Managers;
using UnityEngine;

namespace TalesOfRadiance.Scripts.Battle.BattleComponents.RuntimeClass
{
    public class RuntimeSkill
    {
        public SkillTemplate Template;
        public int Cooldown;

        public RuntimeSkill(int skillID)
        {
            Template = ConfigManager.Instance.GetSkillTemplateByID(skillID);
            Cooldown = Template.InitCd;
        }

        public SkillTemplate ExecuteSkill()
        {
            Cooldown = Template.Cd;
            return Template;
        }
    }
}