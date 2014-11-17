using System;
using System.Runtime.InteropServices;

namespace Game2048.Infrastructure.Interfaces
{
    /// <summary>
    /// Ole Service Provider interface.
    /// </summary>
    public interface IOleServiceProvider
    {
        /// <summary>
        /// Queries the service.
        /// </summary>
        /// <param name="guidService">The service id.</param>
        /// <param name="riid">Caller unique idenifier.</param>
        /// <param name="ppvObject">Caller address.</param>
        /// <returns></returns>
        [PreserveSig]
        int QueryService([In] ref Guid guidService, [In] ref Guid riid, [MarshalAs(UnmanagedType.IDispatch)] out object ppvObject);
    }
}
