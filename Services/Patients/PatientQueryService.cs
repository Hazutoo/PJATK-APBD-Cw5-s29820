using Microsoft.EntityFrameworkCore;
using PJATK_APBD_Hospital.Contracts.Patients;
using PJATK_APBD_Hospital.Database;

namespace PJATK_APBD_Hospital.Services.Patients;

public sealed class PatientQueryService(HospitalDbContext dbContext) : IPatientQueryService
{
    public async Task<IReadOnlyList<PatientResponse>> GetPatientsAsync(
        string? search,
        CancellationToken cancellationToken)
    {
        var patients = dbContext.Patients.AsNoTracking();
        var normalizedSearch = search?.Trim();

        if (!string.IsNullOrEmpty(normalizedSearch))
        {
            var likePattern = $"%{normalizedSearch}%";

            patients = patients.Where(patient =>
                EF.Functions.Like(patient.FirstName, likePattern) ||
                EF.Functions.Like(patient.LastName, likePattern));
        }

        return await patients
            .OrderBy(patient => patient.Pesel)
            .Select(patient => new PatientResponse(
                patient.Pesel,
                patient.FirstName,
                patient.LastName,
                patient.Age,
                patient.Sex ? "Male" : "Female",
                patient.Admissions
                    .OrderBy(admission => admission.Id)
                    .Select(admission => new AdmissionResponse(
                        admission.Id,
                        admission.AdmissionDate,
                        admission.DischargeDate,
                        new WardResponse(
                            admission.Ward.Id,
                            admission.Ward.Name,
                            admission.Ward.Description)))
                    .ToList(),
                patient.BedAssignments
                    .OrderBy(assignment => assignment.Id)
                    .Select(assignment => new BedAssignmentResponse(
                        assignment.Id,
                        assignment.From,
                        assignment.To,
                        new BedResponse(
                            assignment.Bed.Id,
                            new BedTypeResponse(
                                assignment.Bed.BedType.Id,
                                assignment.Bed.BedType.Name,
                                assignment.Bed.BedType.Description),
                            new RoomResponse(
                                assignment.Bed.Room.Id,
                                assignment.Bed.Room.HasTv,
                                new WardResponse(
                                    assignment.Bed.Room.Ward.Id,
                                    assignment.Bed.Room.Ward.Name,
                                    assignment.Bed.Room.Ward.Description)))))
                    .ToList()))
            .ToListAsync(cancellationToken);
    }
}
