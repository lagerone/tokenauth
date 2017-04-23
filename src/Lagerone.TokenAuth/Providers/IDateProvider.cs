using System;

namespace Lagerone.TokenAuth.Providers
{
    internal interface IDateProvider
    {
        DateTime UtcNow { get; }
    }
}
