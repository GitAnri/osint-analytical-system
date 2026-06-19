using Shared.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Shared.Command
{
    public class AddRelationCommandDto
    {

        [JsonIgnore]
        public int IndividualId { get; set; }

        [Required]
        public int RelatedIndividualId { get; set; }

        [Required]
        public RelationType RelationType { get; set; }
    }
}
