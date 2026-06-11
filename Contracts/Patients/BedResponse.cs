namespace PJATK_APBD_Hospital.Contracts.Patients;

public sealed record BedResponse(
    int Id,
    BedTypeResponse BedType,
    RoomResponse Room);
