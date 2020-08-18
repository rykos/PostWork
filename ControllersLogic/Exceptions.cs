using System;
namespace PostWork.ControllersLogic
{
    public class InvalidLoginDataException : Exception { }
    public class InvalidPasswordException : Exception { }
    public class UsernameIsTakenException : Exception { }
    public class EmailIsTakenException : Exception { }
    public class UserDoesNotExistException : Exception { }
    public class PostCreationException : Exception { }
}