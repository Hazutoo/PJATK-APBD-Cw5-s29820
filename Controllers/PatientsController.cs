using Microsoft.AspNetCore.Mvc;
using PJATK_APBD_Hospital.Contracts.BedAssignments;
using PJATK_APBD_Hospital.Contracts.Patients;
using PJATK_APBD_Hospital.Services.BedAssignments;
using PJATK_APBD_Hospital.Services.Patients;

namespace PJATK_APBD_Hospital.Controllers;

[ApiController]
[Route("api/patients")]
public sealed class PatientsController(
    IPatientQueryService patientQueryService,
    IBedAssignmentService bedAssignmentService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<PatientResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PatientResponse>>> GetPatients(
        [FromQuery] string? search,
        CancellationToken cancellationToken)
    {
        var patients = await patientQueryService.GetPatientsAsync(search, cancellationToken);
        return Ok(patients);
    }

    [HttpPost("{pesel}/bedassignments")]
    [ProducesResponseType(typeof(AssignBedResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AssignBedResponse>> AssignBed(
        [FromRoute] string pesel,
        [FromBody] AssignBedRequest request,
        CancellationToken cancellationToken)
    {
        var createdAssignment = await bedAssignmentService.AssignBedAsync(
            pesel,
            request,
            cancellationToken);

        return Created(
            $"/api/patients/{pesel}/bedassignments/{createdAssignment.Id}",
            createdAssignment);
    }
}
