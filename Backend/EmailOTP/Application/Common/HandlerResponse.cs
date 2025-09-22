namespace Application.Common;

public class HandlerResponse
{
    public bool IsSuccess { get; private set; }
    public OtpStatus StatusCode { get; private set; }
    public string ErrorMessage { get; private set; }

    private HandlerResponse(bool isSuccess, OtpStatus otpStatus, string errorMessage)
    {
        IsSuccess = isSuccess;
        StatusCode = otpStatus;
        ErrorMessage = errorMessage;
    }

    public static HandlerResponse Success()
    {
        return new HandlerResponse(true, OtpStatus.Ok, string.Empty);
    }

    public static HandlerResponse Failed(OtpStatus otpStatus, string errorMessage)
    {
        return new HandlerResponse(false, otpStatus, errorMessage);
    }
}

public enum OtpStatus
{
    Ok,
    InvalidEmail,
    InvalidEmailDomain,
    InvalidOtp,
    NotFound,
    Expired,
    TooManyAttempts,
    EmailFailed,
}