using AgeSmartVocabulary.Data;


namespace AgeSmartVocabulary.Services
{
    /// <summary>
    /// Quick tester for API services
    /// Use this in MainPage.xaml.cs to verify everything works
    /// </summary>
    public class ServiceTester
    {
        public static async Task TestAllServicesAsync()
        {
            System.Diagnostics.Debug.WriteLine("=== Starting Service Tests ===");

            // Test 1: Datamuse API
            //var datamuseApi = new DatamuseApiService();
            //var words = await datamuseApi.GetRandomWordsAsync(10);
            //System.Diagnostics.Debug.WriteLine($"✅ Datamuse: Fetched {words.Count} words");

            //if (words.Count > 0)
            //{
            //    var firstWord = words.First();
            //    var freq = datamuseApi.ExtractFrequency(firstWord);
            //    System.Diagnostics.Debug.WriteLine($"   Example: {firstWord.Word} (frequency: {freq})");
            //}

            //// Test 2: Dictionary API
            //var dictionaryApi = new DictionaryApiService();
            //var definition = await dictionaryApi.GetWordDefinitionAsync("happy");
            //System.Diagnostics.Debug.WriteLine($"✅ Dictionary: Retrieved definition for 'happy'");

            //if (definition != null)
            //{
            //    var (meaning, example, pos) = dictionaryApi.GetBestDefinition(definition);
            //    System.Diagnostics.Debug.WriteLine($"   Meaning: {meaning}");
            //}

            //// Test 3: Database
            //var database = new DatabaseService();
            //var profile = await database.GetUserProfileAsync();
            //System.Diagnostics.Debug.WriteLine($"✅ Database: Connection successful");

            //// Test 4: Full Sync (only 5 words for quick test)
            ////var syncService = new WordSyncService(datamuseApi, dictionaryApi, database);
            ////System.Diagnostics.Debug.WriteLine("🔄 Starting quick sync (5 words)...");

            ////var result = await syncService.SyncWordsAsync(5);
            ////System.Diagnostics.Debug.WriteLine($"✅ Sync Complete: {result}");

            System.Diagnostics.Debug.WriteLine("=== All Tests Complete ===");
        }
    }
}
