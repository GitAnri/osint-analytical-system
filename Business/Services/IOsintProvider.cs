using Shared.Models;
using Shared.Command;

namespace Business.Services
{
    public interface IOsintProvider
    {
        Task<ScraperRawDataDto> FetchDataAsync(string firstName, string lastName);
    }

    public class ScraperRawDataDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string PersonalNumber { get; set; } = null!;
        public Gender Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public CityEnum City { get; set; }
        public List<string> PhoneNumbers { get; set; } = new();
        public string SourceUrl { get; set; } = null!;
        public int RiskScore { get; set; }
        public List<ScrapedRelationDto> Relations { get; set; } = new();
    }

    public class ScrapedRelationDto
    {
        public string TargetPersonalNumber { get; set; } = null!;
        public string TargetFirstName { get; set; } = "Unknown";
        public string TargetLastName { get; set; } = "Unknown";
        public RelationType RelationType { get; set; }
    }
}