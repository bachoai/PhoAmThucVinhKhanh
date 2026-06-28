using SQLite;

namespace Quan4CulinaryTourism.Mobile.LocalDatabase;

public class LocalDbContext
{
    private SQLiteAsyncConnection? _connection;

    public async Task<SQLiteAsyncConnection> GetConnectionAsync()
    {
        if (_connection is not null)
        {
            return _connection;
        }

        SQLitePCL.Batteries_V2.Init();
        var databasePath = Path.Combine(FileSystem.AppDataDirectory, "quan4-tourism.db3");
        _connection = new SQLiteAsyncConnection(databasePath);

        await _connection.CreateTableAsync<LocalCategory>();
        await _connection.CreateTableAsync<LocalPoi>();
        await _connection.CreateTableAsync<LocalPoiDetail>();
        await _connection.CreateTableAsync<LocalPoiAudio>();
        await _connection.CreateTableAsync<LocalMapPack>();
        await _connection.CreateTableAsync<LocalCacheMetadata>();

        return _connection;
    }
}
