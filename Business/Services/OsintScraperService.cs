using DAL.Infrastructure;
using DAL.Repositories;
using Shared.Command;
using Shared.Models;

namespace Business.Services
{
    public class OsintScraperService : IOsintScraperService
    {
        private readonly IOsintProvider _osintProvider;
        private readonly IIndividualService _individualService;
        private readonly IRelationService _relationService;
        private readonly IIndividualRepository _individualRepo;
        private readonly IUnitOfWork _unitOfWork;

        public OsintScraperService(
            IOsintProvider osintProvider,
            IIndividualService individualService,
            IRelationService relationService,
            IIndividualRepository individualRepo,
            IUnitOfWork unitOfWork)
        {
            _osintProvider = osintProvider;
            _individualService = individualService;
            _relationService = relationService;
            _individualRepo = individualRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<int> GatherAndSavePublicDataAsync(string firstName, string lastName)
        {
            var scrapedData = await _osintProvider.FetchDataAsync(firstName, lastName);

            var scrapedDto = new CreateIndividualCommandDto
            {
                FirstName = scrapedData.FirstName,
                LastName = scrapedData.LastName,
                Gender = scrapedData.Gender,
                PersonalNumber = scrapedData.PersonalNumber,
                DateOfBirth = scrapedData.DateOfBirth,
                City = scrapedData.City,
                PhoneNumbers = scrapedData.PhoneNumbers.Select(num => new CreatePhoneNumberDto
                {
                    Number = num,
                    Type = PhoneNumberType.Mobile
                }).ToList()
            };

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var newId = await _individualService.AddIndividualAsync(scrapedDto);

                if (scrapedData.Relations != null && scrapedData.Relations.Any())
                {
                    foreach (var scrapedRelation in scrapedData.Relations)
                    {
                        var targetInDb = await _individualRepo.GetByPersonalNumberAsync(scrapedRelation.TargetPersonalNumber);

                        int relatedIndividualId;

                        if (targetInDb != null)
                        {
                            relatedIndividualId = targetInDb.Id;
                        }
                        else
                        {
                            var ghostProfileDto = new CreateIndividualCommandDto
                            {
                                FirstName = scrapedRelation.TargetFirstName,
                                LastName = scrapedRelation.TargetLastName,
                                Gender = Gender.Male,
                                PersonalNumber = scrapedRelation.TargetPersonalNumber,
                                DateOfBirth = DateTime.UtcNow.AddYears(-30),
                                City = CityEnum.Tbilisi,
                                PhoneNumbers = new List<CreatePhoneNumberDto>()
                            };

                            relatedIndividualId = await _individualService.AddIndividualAsync(ghostProfileDto);
                        }

                        await _relationService.AddRelationAsync(new Relation
                        {
                            IndividualId = newId,
                            RelatedIndividualId = relatedIndividualId,
                            RelationType = scrapedRelation.RelationType
                        });
                    }
                }

                var individual = await _individualRepo.GetByIdAsync(newId);
                if (individual != null)
                {
                    int calculatedRiskScore = 10;

                    if (!scrapedDto.PhoneNumbers.Any())
                    {
                        calculatedRiskScore += 25;
                    }

                    if (individual.Relations != null)
                    {
                        calculatedRiskScore += (individual.Relations.Count * 15);
                    }

                    individual.ScrapedAt = DateTime.UtcNow;
                    individual.SourceUrl = scrapedData.SourceUrl;
                    individual.RiskScore = Math.Min(calculatedRiskScore, 100);

                    await _individualRepo.UpdateAsync(individual);
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
                return newId;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}