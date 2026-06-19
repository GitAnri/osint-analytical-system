using Shared.Models;
using Shared.Command;

namespace Business.Services
{
    public class MockOsintProvider : IOsintProvider
    {
        public async Task<ScraperRawDataDto> FetchDataAsync(string firstName, string lastName)
        {
            await Task.Delay(1500);
            var random = new Random();

            var result = new ScraperRawDataDto
            {
                FirstName = firstName,
                LastName = lastName,
                PersonalNumber = random.NextInt64(10000000000, 99999999999).ToString(),
                Gender = Gender.Male,
                DateOfBirth = DateTime.UtcNow.AddYears(-random.Next(18, 60)),
                City = CityEnum.Tbilisi,
                PhoneNumbers = new List<string> { "599" + random.Next(100000, 999999).ToString() },
                SourceUrl = $"https://public-registry.gov.ge/search?q={firstName}+{lastName}",
                RiskScore = random.Next(10, 85),
                Relations = new List<ScrapedRelationDto>()
            };

            int connectionCount = random.Next(2, 6);
            string[] mockNames = { "Giorgi", "Nino", "Levan", "Mariam", "Dato", "Sopo", "Luka", "Ana" };
            string[] mockSurnames = { "Maisuradze", "Kakhidze", "Gelashvili", "Gogelia", "Jikia" };

            for (int i = 0; i < connectionCount; i++)
            {
                result.Relations.Add(new ScrapedRelationDto
                {
                    TargetPersonalNumber = random.NextInt64(10000000000, 99999999999).ToString(),
                    TargetFirstName = mockNames[random.Next(mockNames.Length)],
                    TargetLastName = mockSurnames[random.Next(mockSurnames.Length)],
                    RelationType = (RelationType)random.Next(0, 4)
                });
            }

            return result;
        }
    }
}