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
            WeatherDiscard,
            Skill,
            SkillImm = 5000,
            Item = 5001,
            CommandStage = 10000,
            Debut,
            WithDrawPokemon,
            Immediately,
        }
    }
}