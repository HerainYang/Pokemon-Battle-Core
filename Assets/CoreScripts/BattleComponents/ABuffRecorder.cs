
namespace CoreScripts.BattleComponents
{
    public abstract class ABuffRecorder
    {
        public int EffectLastRound;
        public bool IsAttribute;

        public IBattleEntity Source;
        public IBattleEntity Target;
        
        public ASkillTemplate Template;

        public bool DeletePending = false;
    }
}