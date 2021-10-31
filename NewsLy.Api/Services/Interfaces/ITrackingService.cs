namespace NewsLy.Api.Services.Interfaces
{
    public interface ITrackingService
    {
        string GenerateTrackingId();
        string DetectCreateAndReplaceTrackings(string inputString);
    }
}