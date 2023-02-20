using CoreScripts.BattleComponents;
using PokemonDemo.Scripts.BattlePlayables.Skills;
using PokemonDemo.Scripts.Enum;
using PokemonDemo.Scripts.PokemonLogic.PokemonData;

namespace PokemonDemo.Scripts.PokemonLogic
{
    public class PokemonBuffRecorder : ABuffRecorder
    {
        public int SourceIndex;
        public int TargetIndex;
        
        
        
        public bool IsWeather;

        public float ChangeFactor;
        public PokemonType SType;
        public Weather WeatherType;

        public CommonSkillTemplate ForbiddenCommonSkill;

        public int[] SkillTargets;

        public PokemonBuffRecorder(Pokemon source, Pokemon target, CommonSkillTemplate buffTemplate, bool isAttribute, bool isWeather)
        {
            Source = source;
            Target = target;
            Template = buffTemplate;
            EffectLastRound = buffTemplate.EffectRound;
            IsAttribute = isAttribute;
            IsWeather = isWeather;
        }
    }
}