using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Square9.SecurityDisplay.Models
{
    public class ApiException : Exception
    {
        public ApiException()
        { }
        public ApiException(String message) : base(message)
        { }
        public ApiException(String message, Exception inner) : base(message, inner)
        { }
    }
    public class ArchivePermissionsException : Exception
    {
        public ArchivePermissionsException()
        { }
        public ArchivePermissionsException(String message)
            : base(message)
        { }
    }
    public class LicenseExpiredException : Exception
    {
        public LicenseExpiredException()
        { }
        public LicenseExpiredException(String message)
            : base(message)
        { }
    }
    public class NotFoundException : Exception
    {
        public NotFoundException() { }
        public NotFoundException(String message) : base(message) { }
    }
    public class AllLicensesInUseException : Exception
    {
        public AllLicensesInUseException()
        { }
        public AllLicensesInUseException(String message)
            : base(message)
        { }
    }
    public class ServerNotFoundException : Exception
    {
        public ServerNotFoundException()
        { }
        public ServerNotFoundException(String message) : base(message)
        { }
        public ServerNotFoundException(String message, Exception inner) : base(message, inner)
        { }

    }
}