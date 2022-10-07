using System;

namespace NinUtilsNorway
{
    /// <summary>
    /// Used for mocking date times
    /// </summary>
    public interface IDateTimeNowProvider
    {
        /// <summary>
        /// Retrieves the Today value (DateTime.Today for example). Useful for mocking / unit testing.
        /// </summary>
        /// <returns></returns>
        DateTime GetToday(); 
    }
}