using SQLite;
using AgeSmartVocabulary.Models;

namespace AgeSmartVocabulary.Data
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection _database;
        private readonly string _dbPath;

        public DatabaseService()
        {
            _dbPath = Path.Combine(
                FileSystem.AppDataDirectory,
                "agesmartvocabulary.db3"
            );
        }

        private async Task InitAsync()
        {
            if (_database != null)
                return;

            _database = new SQLiteAsyncConnection(_dbPath);

            // Only create tables for user data and review tracking
            await _database.CreateTableAsync<UserProfile>();
            await _database.CreateTableAsync<ReviewSchedule>();
        }

        // UserProfile operations
        public async Task<UserProfile> GetUserProfileAsync()
        {
            await InitAsync();
            return await _database.Table<UserProfile>().FirstOrDefaultAsync();
        }

        public async Task<int> SaveUserProfileAsync(UserProfile profile)
        {
            await InitAsync();
            if (profile.Id != 0)
                return await _database.UpdateAsync(profile);
            else
                return await _database.InsertAsync(profile);
        }

        // ReviewSchedule operations
        public async Task<int> SaveReviewScheduleAsync(ReviewSchedule schedule)
        {
            await InitAsync();
            var existing = await _database.Table<ReviewSchedule>()
                .Where(r => r.WordText == schedule.WordText)
                .FirstOrDefaultAsync();

            if (existing != null)
            {
                schedule.Id = existing.Id;
                return await _database.UpdateAsync(schedule);
            }
            return await _database.InsertAsync(schedule);
        }

        public async Task<ReviewSchedule> GetTodayReviewAsync()
        {
            await InitAsync();
            return await _database.Table<ReviewSchedule>()
                .Where(r => r.NextReviewDate <= DateTime.Today && !r.IsKnown)
                .OrderBy(r => r.NextReviewDate)
                .FirstOrDefaultAsync();
        }

        public async Task<List<ReviewSchedule>> GetAllPendingReviewsAsync()
        {
            await InitAsync();
            return await _database.Table<ReviewSchedule>()
                .Where(r => r.NextReviewDate <= DateTime.Today && !r.IsKnown)
                .ToListAsync();
        }

        public async Task<List<string>> GetSeenWordsAsync()
        {
            await InitAsync();
            var schedules = await _database.Table<ReviewSchedule>().ToListAsync();
            return schedules.Select(s => s.WordText).ToList();
        }

        public async Task<bool> HasSeenWordAsync(string word)
        {
            await InitAsync();
            var existing = await _database.Table<ReviewSchedule>()
                .Where(r => r.WordText == word)
                .FirstOrDefaultAsync();
            return existing != null;
        }

        /// <summary>
        /// Clear all review history
        /// </summary>
        public async Task ClearAllReviewsAsync()
        {
            await InitAsync();
            await _database.DeleteAllAsync<ReviewSchedule>();
            System.Diagnostics.Debug.WriteLine("✓ All review history cleared from database");
        }
    }
}