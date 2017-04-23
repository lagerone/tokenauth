using System;

namespace Lagerone.TokenAuth.Providers
{
    internal class DateProvider : IDateProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}