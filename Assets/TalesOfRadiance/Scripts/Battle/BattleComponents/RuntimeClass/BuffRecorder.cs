using CoreScripts.BattleComponents;
using CoreScripts.Managers;

namespace TalesOfRadiance.Scripts.Battle.BattleComponents.RuntimeClass
{
    public class BuffRecorder : ABuffRecorder
    {
        public int AccumulateDamage;
        public BuffRecorder(IBattleEntity source, IBattleEntity target, SkillTemplate buffTemplate, bool isAttribute)
        {
            Source = source;
            Target = target;
            Template = buffTemplate;
            EffectLastRound = buffTemplate.BuffLastRound;
            IsAttribute = isAttribute;
        }
    }
}