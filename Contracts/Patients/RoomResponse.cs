namespace PJATK_APBD_Hospital.Contracts.Patients;

public sealed record RoomResponse(
    string Id,
    bool HasTv,
    WardResponse Ward);
