using System;

namespace DevConsole.Exceptions;

public class InvalidConfigurationException : Exception
{
    public InvalidConfigurationException(string message) : base(message) {}
}