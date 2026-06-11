namespace PJATK_APBD_Hospital.Contracts.Common;

public sealed record ErrorResponse(
    int StatusCode,
    string Error,
    string Message);
