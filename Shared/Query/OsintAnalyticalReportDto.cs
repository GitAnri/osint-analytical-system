namespace Shared.Query
{
    public class OsintAnalyticalReportDto
    {
        public IndividualGetResponseDto TargetProfile { get; set; } = null!;
        public int TotalKnownConnections { get; set; }
        public string NetworkInfluenceLevel { get; set; } = string.Empty;
        public List<string> FlaggedVulnerabilities { get; set; } = new();
    }
}