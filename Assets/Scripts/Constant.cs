public static class Constant
{
    public struct EventKey
    {
        public static readonly string HpChange = "HP_CHANGE";
        public static readonly string CommandStageStart = "COMMAND_STAGE_START";
        public static readonly string RequestLoadPokemonSkill = "REQUEST_LOAD_POKEMON_SKILL";
        public static readonly string CommandStageEnd = "COMMAND_STAGE_END";
        public static readonly string BattleCommandSent = "BATTLE_COMMAND_SENT";
        public static readonly string PokemonFaint = "POKEMON_FAINT";
        public static readonly string BattlePokemonForceChangeCommandSent = "BATTLE_POKEMON_FORCE_CHANGE_COMMAND_SENT";
        public static readonly string HeartBeatSent = "HEARTBEAT_SENT";
        public static readonly string RequestSentPokemonOnStage = "REQUEST_SENT_POKEMON_ON_STAGE";
    }
    
    public struct BuffExecutionTimeKey
    {
        public static readonly string BeforeMove = "BEFORE_MOVE";
        public static readonly string BeforeApplyDamage = "BEFORE_APPLY_DAMAGE";
        public static readonly string BeforeTakingDamage = "BEFORE_TAKING_DAMAGE";
        public static readonly string StartOfRound = "START_OF_ROUND";
        public static readonly string EndOfRound = "END_OF_ROUND";
        public static readonly string GettingSkillAccuracy = "GETTING_SKILL_ACCURACY";
        public static readonly string CalculatingHit = "CALCULATING_HIT";
        public static readonly string CalculatingCriticalDamage = "CALCULATING_CRITICAL_DAMAGE";
        public static readonly string CalculatingSkillPower = "CALCULATING_SKILL_POWER";
        public static readonly string CalculatingFinalDamage = "CALCULATING_FINAL_DAMAGE";
        public static readonly string AfterTakingDamage = "AFTER_TAKING_DAMAGE";
        public static readonly string AfterHealDone = "AFTER_HEAL_DONE";
        public static readonly string AfterDebut = "AFTER_DEBUT";
        public static readonly string BeforeAddBuff = "BEFORE_ADD_BUFF";
        public static readonly string OnAddBuff = "ON_ADD_BUFF";
        public static readonly string BeforeWithdraw = "BEFORE_WITHDRAW";
        public static readonly string WhenGettingTarget = "WHEN_GETTING_TARGET";
        public static readonly string BeforeRequirePokemonCommand = "BEFORE_REQUIRE_POKEMON_COMMAND";
        public static readonly string BeforeLoadPokemonSkill = "BEFORE_LOAD_POKEMON_SKILL";
        

        public static readonly string GettingSpecialAttack = "GETTING_SPECIAL_ATTACK";

        public static readonly string OnWeatherChange = "ON_WEATHER_CHANGE";

        public static readonly string None = "NONE";
    }
    
    public struct PlayerInfo
    {
        public static readonly string LocalPlayerID = "p1";
    }
    
    public struct GameConfig
    {
        public static readonly float CriticalDamageIncrease = 2.25f;
    }
}