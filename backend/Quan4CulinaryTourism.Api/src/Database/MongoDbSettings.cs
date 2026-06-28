namespace Quan4CulinaryTourism.Api.Database;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = "mongodb://localhost:27017";
    public string DatabaseName { get; set; } = "quan4_culinary_tourism";
}

public class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpireMinutes { get; set; } = 120;
}

public class UploadSettings
{
    public string UploadPath { get; set; } = "wwwroot/uploads";
    public int MaxImageSizeMb { get; set; } = 5;
    public int MaxAudioSizeMb { get; set; } = 20;
}

public class DefaultAdminSettings
{
    public string Email { get; set; } = "admin@quan4tourism.local";
    public string Password { get; set; } = "Admin@123456";
    public string FullName { get; set; } = "System Admin";
}

public class CorsSettings
{
    public List<string> AllowedOrigins { get; set; } = [];
}

public class TextToSpeechSettings
{
    public bool Enabled { get; set; } = true;
    public string PythonCommand { get; set; } = "python";
    public string ScriptPath { get; set; } = "tools/tts_generate.py";
    public string DefaultVoice { get; set; } = "vi";
    public string Rate { get; set; } = "+0%";
    public int TimeoutSeconds { get; set; } = 90;
}
