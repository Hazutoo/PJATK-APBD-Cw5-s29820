namespace PJATK_APBD_Hospital.Contracts.BedAssignments;

public sealed record AssignBedResponse(
    int Id,
    string PatientPesel,
    int BedId,
    DateTime From,
    DateTime? To);
