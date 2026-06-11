namespace PJATK_APBD_Hospital.Contracts.Patients;

public sealed record AdmissionResponse(
    int Id,
    DateTime AdmissionDate,
    DateTime? DischargeDate,
    WardResponse Ward);
