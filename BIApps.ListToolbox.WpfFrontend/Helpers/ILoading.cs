using System.ComponentModel;
using System.Threading;

namespace BIApps.ListToolbox.WpfFrontend.Helpers {

    /// <summary>
    /// Delegate used to notify when <see cref="ILoading"/> has changed value.
    /// </summary>
    public delegate void LoadingChanged();

    /// <summary>
    /// A global helper object used to display whether the application is doing background work or not. This
    /// should be instantiated in a singleton scope so as to be shared around all the ViewModels.
    /// </summary>
    public interface ILoading : INotifyPropertyChanged {

        /// <summary>
        /// Event used to notify when loading has started or finished.
        /// </summary>
        event LoadingChanged LoadingChanged;

        /// <summary>
        /// The <see cref="CancellationTokenSource"/> used to cancel any pending tasks that are occurring.
        /// </summary>
        CancellationTokenSource TokenSource { get; set; }

        /// <summary>
        /// Gets the <see cref="CancellationToken"/> from the <see cref="CancellationTokenSource"/>, which
        /// is used to pass into the methods which can be cancelled.
        /// </summary>
        CancellationToken Token { get; }

        /// <summary>
        /// Gets the number of tasks that are currently running in the background.
        /// </summary>
        int TaskCount { get; }

        /// <summary>
        /// Gets whether the application is currently doing background work or not.
        /// </summary>
        bool Value { get; }

        /// <summary>
        /// Lets the <see cref="ILoading"/> object know that a new task is starting and increments the count.
        /// </summary>
        void Start();

        /// <summary>
        /// Lets the <see cref="ILoading"/> object know that a task has finished and decrements the count.
        /// </summary>
        void End();

        /// <summary>
        /// Cancels any running tasks.
        /// </summary>
        void Cancel();
    }
}
