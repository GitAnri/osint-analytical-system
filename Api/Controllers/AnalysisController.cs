using Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Query;

namespace Controllers
{
    [ApiController]
    [Route("api/analysis")]
    [Authorize(Policy = "AnalystLevel")]
    public class AnalysisController : ControllerBase
    {
        private readonly IIndividualService _individualService;
        private readonly IOsintScraperService _scraperService;

        public AnalysisController(IIndividualService individualService, IOsintScraperService scraperService)
        {
            _individualService = individualService;
            _scraperService = scraperService;
        }

        [HttpPost("gather")]
        public async Task<IActionResult> GatherIntelligence(string firstName, string lastName)
        {
            var id = await _scraperService.GatherAndSavePublicDataAsync(firstName, lastName);
            return Ok(new { Message = "Intelligence successfully gathered.", IndividualId = id });
        }

        [HttpGet("{id}/report")]
        public async Task<IActionResult> GenerateOsintReport(int id)
        {
            var individual = await _individualService.GetByIdAsync(id);
            if (individual == null) return NotFound(new { Message = "Target not found in database." });

            var report = new OsintAnalyticalReportDto
            {
                TargetProfile = individual,
                TotalKnownConnections = individual.Relations.Count,
                NetworkInfluenceLevel = individual.Relations.Count >= 3 ? "High" : "Low",
            };

            if (!individual.PhoneNumbers.Any())
                report.FlaggedVulnerabilities.Add("Target has no publicly linked communication methods (Ghost Profile).");

            if (individual.RiskScore > 75)
                report.FlaggedVulnerabilities.Add("Target exhibits a high automated risk score.");

            return Ok(report);
        }
    }
}