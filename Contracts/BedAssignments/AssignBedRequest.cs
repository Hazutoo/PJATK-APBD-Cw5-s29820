using System.ComponentModel.DataAnnotations;

namespace PJATK_APBD_Hospital.Contracts.BedAssignments;

public sealed record AssignBedRequest(
    DateTime From,
    DateTime? To,
    string BedType,
    string Ward) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (From == default)
        {
            yield return new ValidationResult(
                "Field 'from' is required and must be a valid date.",
                [nameof(From)]);
        }

        if (To is not null && To <= From)
        {
            yield return new ValidationResult(
                "Field 'to' must be later than field 'from'.",
                [nameof(To), nameof(From)]);
        }

        if (string.IsNullOrWhiteSpace(BedType))
        {
            yield return new ValidationResult(
                "Field 'bedType' is required.",
                [nameof(BedType)]);
        }

        if (string.IsNullOrWhiteSpace(Ward))
        {
            yield return new ValidationResult(
                "Field 'ward' is required.",
                [nameof(Ward)]);
        }
    }
}
