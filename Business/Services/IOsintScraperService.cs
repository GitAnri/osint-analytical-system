namespace Business.Services
{
    public interface IOsintScraperService
    {
        Task<int> GatherAndSavePublicDataAsync(string firstName, string lastName);
    }
}
