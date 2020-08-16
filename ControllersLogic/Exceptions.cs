using System;
namespace PostWork.ControllersLogic
{
    public class InvalidLoginDataException : Exception { }
    public class UsernameIsTakenException : Exception { }
    public class EmailIsTakenException : Exception { }
}