using Microsoft.Extensions.Configuration;

namespace Modules.Common.Database
{
    public static class DbConfiguration
    {
        private const string DefaultFileName = "TodoApp";
        private const string DefaultFolderName = "TodoAppDatabase";

        private static IConfiguration? _configuration;

        public static string DatabasePath { get; private set; }
        public static string ConnectionString => $"Data Source={DatabasePath};";

        static DbConfiguration()
        {
            var directory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            directory = Path.Combine(directory, DefaultFolderName);
            var filenameWithExtension = $"{DefaultFileName}.{Constants.DatabaseFileExtension}";

            DatabasePath = Path.Combine(directory, filenameWithExtension);
        }

        public static void Initialize(IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);
            _configuration = configuration;

            var directory = _configuration[Constants.ConfigKeys.DatabaseDirectory];
            var filename = _configuration[Constants.ConfigKeys.DatabaseFileName];

            if (string.IsNullOrWhiteSpace(filename))
            {
                filename = DefaultFileName;
            }

            var filenameWithExtension = $"{filename}.{Constants.DatabaseFileExtension}";

            if (string.IsNullOrWhiteSpace(directory))
            {
                directory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                directory = Path.Combine(directory, DefaultFolderName);
            }

            ValidatePath(directory);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!IsFileNameValid(filenameWithExtension))
            {
                throw new ApplicationException($"{Constants.ConfigKeys.DatabaseFileName} is not a valid filename!");
            }

            DatabasePath = Path.Combine(directory, filenameWithExtension);
        }

        private static void ValidatePath(string directory)
        {
            // Throws exception if the path is invalid
            Path.GetFullPath(directory);

            if (!Path.IsPathRooted(directory))
            {
                throw new ApplicationException($"{Constants.ConfigKeys.DatabaseDirectory} must be a valid absolute path!");
            }
        }

        private static bool IsFileNameValid(string fileName)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            foreach (var invalidChar in invalidChars)
            {
                if (fileName.Contains(invalidChar))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
