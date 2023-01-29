namespace PokemonDemo
{
    public static class Constant
    {
        public struct EventKey
        {
            public const string HpChange = "HP_CHANGE";
            public const string CommandStageStart = "COMMAND_STAGE_START";
            public const string RequestLoadPokemonSkill = "REQUEST_LOAD_POKEMON_SKILL";
            public const string CommandStageEnd = "COMMAND_STAGE_END";
            public const string BattleCommandSent = "BATTLE_COMMAND_SENT";
            public const string PokemonFaint = "POKEMON_FAINT";
            public const string BattlePokemonForceChangeCommandSent = "BATTLE_POKEMON_FORCE_CHANGE_COMMAND_SENT";
            public const string HeartBeatSent = "HEARTBEAT_SENT";
            public const string RequestSentPokemonOnStage = "REQUEST_SENT_POKEMON_ON_STAGE";
            // public const string RequestUseItemOnPokemon = "REQUEST_USE_ITEM_ON_POKEMON";
        }
    
        public struct UIEventKey
        {
            public const string ClosePokemonSelectWindow = "CLOSE_POKEMON_SELECT_WINDOW";
            public const string CloseSkillSelectWindow = "CLOSE_SKILL_SELECT_WINDOW";
        }
    
        public struct BuffExecutionTimeKey
        {
            public const string BeforeMove = "BEFORE_MOVE";
            public const string BeforeApplyDamage = "BEFORE_APPLY_DAMAGE";
            public const string BeforeTakingDamage = "BEFORE_TAKING_DAMAGE";
            public const string StartOfRound = "START_OF_ROUND";
            public const string EndOfRound = "END_OF_ROUND";
            public const string GettingSkillAccuracy = "GETTING_SKILL_ACCURACY";
            public const string CalculatingHit = "CALCULATING_HIT";
            public const string CalculatingCriticalDamage = "CALCULATING_CRITICAL_DAMAGE";
            public const string CalculatingSkillPower = "CALCULATING_SKILL_POWER";
            public const string CalculatingFinalDamage = "CALCULATING_FINAL_DAMAGE";
            public const string AfterTakingDamage = "AFTER_TAKING_DAMAGE";
            public const string AfterHealDone = "AFTER_HEAL_DONE";
            public const string AfterDebut = "AFTER_DEBUT";
            public const string BeforeAddBuff = "BEFORE_ADD_BUFF";
            public const string OnAddBuff = "ON_ADD_BUFF";
            public const string BeforeWithdraw = "BEFORE_WITHDRAW";
            public const string WhenGettingTarget = "WHEN_GETTING_TARGET";
            public const string BeforeRequirePokemonCommand = "BEFORE_REQUIRE_POKEMON_COMMAND";
            public const string BeforeLoadPokemonSkill = "BEFORE_LOAD_POKEMON_SKILL";
            public const string GettingSpecialAttack = "GETTING_SPECIAL_ATTACK";
            public const string OnWeatherChange = "ON_WEATHER_CHANGE";
            public const string None = "NONE";
        }
    
        public struct PlayerInfo
        {
            public const string LocalPlayerID = "p1";
        }
    
        public struct GameConfig
        {
            public const float CriticalDamageIncrease = 2.25f;
        }
    }
}