namespace PJATK_APBD_Hospital.Contracts.Patients;

public sealed record PatientResponse(
    string Pesel,
    string FirstName,
    string LastName,
    int Age,
    string Sex,
    IReadOnlyList<AdmissionResponse> Admissions,
    IReadOnlyList<BedAssignmentResponse> BedAssignments);
