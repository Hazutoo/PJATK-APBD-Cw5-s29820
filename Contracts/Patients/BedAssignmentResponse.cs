namespace PJATK_APBD_Hospital.Contracts.Patients;

public sealed record BedAssignmentResponse(
    int Id,
    DateTime From,
    DateTime? To,
    BedResponse Bed);
