using System;
using Enum;
using Managers.BattleMgrComponents.BattlePlayables.Skills;

namespace Managers.BattleMgrComponents.PokemonLogic
{
    public class BuffRecorder
    {
        public bool Available;
        public Pokemon Source;
        public int SourceIndex;
        public Pokemon Target;
        public int TargetIndex;
        public int StartAtRound;
        
        public CommonSkillTemplate Template;
        
        public int EffectLastRound;
        public bool IsAttribute;
        public bool IsWeather;

        public float ChangeFactor;
        public PokemonType SType;
        public Weather WeatherType;

        public CommonSkillTemplate ForbiddenCommonSkill;

        public BuffRecorder(Pokemon source, Pokemon target, CommonSkillTemplate buffTemplate, bool isAttribute, bool isWeather)
        {
            Source = source;
            Target = target;
            Available = true;
            Template = buffTemplate;
            EffectLastRound = Template.EffectRound;
            IsAttribute = isAttribute;
            IsWeather = isWeather;
        }
    }
}