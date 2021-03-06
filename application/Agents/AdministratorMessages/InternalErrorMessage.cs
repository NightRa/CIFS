﻿using System;
using static System.Environment;

namespace Agents.AdministratorMessages
{
    public sealed class InternalErrorMessage : AdministratorMessage
    {
        public Exception Exception { get; }

        public InternalErrorMessage(Exception exception)
        {
            Exception = exception;
        }

        public override string AsString()
        {
            return "Internal error:" + NewLine + Exception;
        }
    }
}
