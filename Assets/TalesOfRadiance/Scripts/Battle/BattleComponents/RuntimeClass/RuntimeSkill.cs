using TalesOfRadiance.Scripts.Battle.Managers;

namespace TalesOfRadiance.Scripts.Battle.BattleComponents.RuntimeClass
{
    public class RuntimeSkill
    {
        public SkillTemplate Template;
        public int Cooldown;

        public RuntimeSkill(int skillID)
        {
            Template = ConfigManager.Instance.GetSkillTemplateByID(0);
            Cooldown = Template.InitCd;
        }

        public SkillTemplate ExecuteSkill()
        {
            Cooldown = Template.Cd;
            return Template;
        }
    }
}