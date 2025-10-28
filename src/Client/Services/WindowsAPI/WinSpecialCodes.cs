namespace Ananke.Services.WindowsAPI;

// https://learn.microsoft.com/en-us/windows/win32/debug/system-error-codes--0-499-"
public enum WindowsErrorCodes 
{
    // Success codes
    ERROR_SUCCESS = 0,
    DNS_REQUEST_PENDING = 9506,
    ERROR_CANCELLED = 1223,
    
    // Error codes
    ERROR_INVALID_PARAMETER = 87,
}