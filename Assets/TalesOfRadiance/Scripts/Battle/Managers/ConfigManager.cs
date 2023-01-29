namespace TalesOfRadiance.Scripts.Battle.Managers
{
    public class ConfigManager
    {
        private static ConfigManager _instance;

        public int[][][] SquadTypeConfig = new[]
        {
            new[]
            {
                new[] { 1, 1, 0 },
                new[] { 0, 0, 1 },
                new[] { 1, 1, 0 }
            },
            new[]
            {
                new[] { 0, 1, 1 },
                new[] { 1, 0, 0 },
                new[] { 0, 1, 1 }
            },
            new[]
            {
                new[] { 0, 0, 1 },
                new[] { 1, 1, 1 },
                new[] { 0, 0, 1 }
            },
            new[]
            {
                new[] { 0, 0, 1 },
                new[] { 1, 1, 1 },
                new[] { 0, 0, 1 }
            },
            new[]
            {
                new[] { 0, 0, 1 },
                new[] { 1, 1, 1 },
                new[] { 0, 0, 1 }
            },
            new[]
            {
                new[] { 0, 0, 1 },
                new[] { 1, 1, 1 },
                new[] { 0, 0, 1 }
            },
        };

        public static ConfigManager Instance
        {
            get { return _instance ??= new ConfigManager(); }
        }
    }
}