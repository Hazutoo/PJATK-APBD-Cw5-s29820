using System.Data;
using Microsoft.EntityFrameworkCore;
using PJATK_APBD_Hospital.Contracts.BedAssignments;
using PJATK_APBD_Hospital.Database;
using PJATK_APBD_Hospital.Database.Entities;
using PJATK_APBD_Hospital.Exceptions;

namespace PJATK_APBD_Hospital.Services.BedAssignments;

public sealed class BedAssignmentService(HospitalDbContext dbContext) : IBedAssignmentService
{
    private static readonly DateTime OpenEndedPeriodLimit = new(9999, 12, 31);

    public async Task<AssignBedResponse> AssignBedAsync(
        string patientPesel,
        AssignBedRequest request,
        CancellationToken cancellationToken)
    {
        patientPesel = patientPesel.Trim();
        var wardName = request.Ward.Trim();
        var bedTypeName = request.BedType.Trim();
        var requestedTo = request.To ?? OpenEndedPeriodLimit;

        var patientExists = await dbContext.Patients
            .AsNoTracking()
            .AnyAsync(patient => patient.Pesel == patientPesel, cancellationToken);

        if (!patientExists)
        {
            throw new NotFoundApiException($"Patient with PESEL '{patientPesel}' was not found.");
        }

        var wardExists = await dbContext.Wards
            .AsNoTracking()
            .AnyAsync(ward => ward.Name == wardName, cancellationToken);

        if (!wardExists)
        {
            throw new NotFoundApiException($"Ward '{wardName}' was not found.");
        }

        var bedTypeExists = await dbContext.BedTypes
            .AsNoTracking()
            .AnyAsync(bedType => bedType.Name == bedTypeName, cancellationToken);

        if (!bedTypeExists)
        {
            throw new NotFoundApiException($"Bed type '{bedTypeName}' was not found.");
        }

        await using var transaction = await dbContext.Database
            .BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

        var bedsInRequestedPlace = dbContext.Beds
            .Where(bed =>
                bed.BedType.Name == bedTypeName &&
                bed.Room.Ward.Name == wardName);

        var anyMatchingBedExists = await bedsInRequestedPlace
            .AnyAsync(cancellationToken);

        if (!anyMatchingBedExists)
        {
            throw new NotFoundApiException(
                $"There is no bed of type '{bedTypeName}' in ward '{wardName}'.");
        }

        var availableBed = await bedsInRequestedPlace
            .Where(bed => !bed.BedAssignments.Any(assignment =>
                assignment.From < requestedTo &&
                (assignment.To == null || assignment.To > request.From)))
            .OrderBy(bed => bed.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (availableBed is null)
        {
            var readableTo = request.To?.ToString("s") ?? "open-ended";

            throw new NotFoundApiException(
                $"No available bed of type '{bedTypeName}' in ward '{wardName}' " +
                $"for period from '{request.From:s}' to '{readableTo}'.");
        }

        var assignmentToCreate = new BedAssignment
        {
            PatientPesel = patientPesel,
            BedId = availableBed.Id,
            From = request.From,
            To = request.To
        };

        dbContext.BedAssignments.Add(assignmentToCreate);
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return new AssignBedResponse(
            assignmentToCreate.Id,
            assignmentToCreate.PatientPesel,
            assignmentToCreate.BedId,
            assignmentToCreate.From,
            assignmentToCreate.To);
    }
}
