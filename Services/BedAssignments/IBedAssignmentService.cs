using PJATK_APBD_Hospital.Contracts.BedAssignments;

namespace PJATK_APBD_Hospital.Services.BedAssignments;

public interface IBedAssignmentService
{
    Task<AssignBedResponse> AssignBedAsync(
        string patientPesel,
        AssignBedRequest request,
        CancellationToken cancellationToken);
}
