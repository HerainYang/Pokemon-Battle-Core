namespace Enum
{
    public enum PokemonType
    {
        Normal,
        Fighting,
        Flying,
        Poison,
        Ground,
        Rock,
        Bug,
        Ghost,
        Steel,
        Fire,
        Water,
        Grass,
        Electric,
        Psychic,
        Ice,
        Dragon,
        Dark,
        Fairy
    }

    public enum PokemonStat
    {
        Attack,
        Defence,
        SpecialAttack,
        SpecialDefence,
        Speed,
        Accuracy,
        Evasion,
        CriticalHit,
    }

    public enum SkillStatus
    {
        Success,
        Fail
    }

    public enum BattleRoundStatus
    {
        Prepare,
        Running,
        Pause,
        End,
        Dead,
    }

    public enum SkillType
    {
        Physical,
        Special,
        Status,
    }

    public enum SkillTargetType
    {
        OneEnemy,
        OneTeammate,
        Self,
        AllEnemy,
        AllTeammate,
        All,
        AllExceptSelf,
        FirstAvailableEnemy,
        FirstEnemy,
        FirstAvailableTeammate,
        FirstTeammate,
        None,
    }

    public enum SkillEffect
    {
        CriticalDamage,
        SuperEffective,
        Effective,
        NotVeryEffective,
        NotEffective,
    }

    public enum CommandRequestType
    {
        Default,
        Skills,
        Pokemons,
        Items,
        Run
    }

    // the higher the index, the higher the priority
    public enum DamageApplyPriority
    {
        Normal,
        Guard,
        MoldBreaker,
        LightningRod,
    }

    public enum MovePriority
    {
        Normal,
        Flinch,
        Paralysis,
    }

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

    public enum Weather
    {
        None,
        HarshSunlight,
        StrongSunLight,
        Rain,
        Sandstorm,
        Hail,
        Snow,
        Fog,
        ExtremelyHarshSunlight,
        StrongWinds,
        ShadowyAura
    }

    public enum Terrain
    {
        Electric,
        Grassy,
        Misty,
        Psychic
    }

    public enum PokemonEssentialSystemManager
    {
        PokemonBattlePanel,
        PokemonMgr
    }

    public enum ItemTarget
    {
        Hp,
        OnePp,
        AllPp,
        Buff,
    }
}