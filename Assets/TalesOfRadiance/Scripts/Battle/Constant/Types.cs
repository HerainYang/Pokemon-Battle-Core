namespace TalesOfRadiance.Scripts.Battle.Constant
{
    public class Types
    {
        public enum PlayablePriority
        {
            None,
            HeartBeat,
            ForceAddPokemon,
            EndOfRound,
            Skill,
            SkillImm = 5000,
            Item = 5001,
            CommandStage = 10000,
            Debut,
            Faint,
            Immediately,
        }

        public enum BuffType
        {
            Negative,
            Neutral,
            Positive
        }

        public enum SkillType
        {
            Passive,
            Active,
            Buff,
            Void
        }

        public enum DamageDonePriority
        {
            Default,
            DawnProtect,
            BurntLotus
        }

        public enum BuffRemovePriority
        {
            Default,
            NormalSteal,
            NormalRemove,
            HolyFire,
            BecomeLotus,
            HeartLotus,
            FeatherProtect,
            Highest,
        }
        
        public enum Position
        {
            Front,
            Mid,
            Back
        }
    }
}