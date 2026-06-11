using PJATK_APBD_Hospital.Contracts.Patients;

namespace PJATK_APBD_Hospital.Services.Patients;

public interface IPatientQueryService
{
    Task<IReadOnlyList<PatientResponse>> GetPatientsAsync(string? search, CancellationToken cancellationToken);
}
