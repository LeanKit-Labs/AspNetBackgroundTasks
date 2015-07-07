using System;
using System.Threading;
using LK.AspNetBackgroundTasks.Internal;

namespace LK.AspNetBackgroundTasks
{
    /// <summary>
    /// A type that tracks background operations and notifies ASP.NET that they are still in progress.
    /// </summary>
    public static class BackgroundTaskManager
    {
        /// <summary>
        /// The background task manager for this app domain.
        /// </summary>
        private static readonly RegisteredTasks Instance = new RegisteredTasks();

        /// <summary>
        /// Gets a cancellation token that is set when ASP.NET is shutting down the app domain.
        /// </summary>
        public static CancellationToken Shutdown { get { return Instance.Shutdown; } }

        /// <summary>
        /// Executes a background operation, registering it with ASP.NET.
        /// </summary>
        /// <param name="operation">The background operation.</param>
        public static void Run(Action operation)
        {
            Instance.Run(operation);
        }
    }
}
