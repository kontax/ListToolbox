using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using BIApps.ListToolbox.WpfFrontend.Annotations;

namespace BIApps.ListToolbox.WpfFrontend.Helpers {


    /// <summary>
    /// A global helper object used to display whether the application is doing background work or not. This
    /// should be instantiated in a singleton scope so as to be shared around all the ViewModels.
    /// </summary>
    public class Loading : ILoading {

        /// <summary>
        /// Event used to notify when loading has started or finished.
        /// </summary>
        public event LoadingChanged LoadingChanged;

        /// <summary>
        /// The <see cref="CancellationTokenSource"/> used to cancel any pending tasks that are occurring.
        /// </summary>
        public CancellationTokenSource TokenSource { get; set; }

        /// <summary>
        /// Gets the <see cref="CancellationToken"/> from the <see cref="CancellationTokenSource"/>, which
        /// is used to pass into the methods which can be cancelled.
        /// </summary>
        public CancellationToken Token { get { return TokenSource.Token; } }

        /// <summary>
        /// Gets the number of tasks that are currently running in the background.
        /// </summary>
        public int TaskCount { get; private set; }

        /// <summary>
        /// Gets whether the application is currently doing background work or not.
        /// </summary>
        public bool Value { get { return TaskCount > 0; } }

        /// <summary>
        /// Instantiates a new <see cref="Loading"/> class with a specified <see cref="CancellationTokenSource"/>.
        /// </summary>
        /// <param name="tokenSource">
        /// The <see cref="CancellationTokenSource"/> used to cancel any tasks as requested by the user.
        /// </param>
        public Loading(CancellationTokenSource tokenSource) {
            TokenSource = tokenSource;
        }

        /// <summary>
        /// Lets the <see cref="ILoading"/> object know that a new task is starting and increments the count.
        /// </summary>
        public void Start() {
            TaskCount++;
            OnPropertyChanged("Value");
            if(LoadingChanged != null) LoadingChanged();
        }

        /// <summary>
        /// Lets the <see cref="ILoading"/> object know that a task has finished and decrements the count.
        /// </summary>
        public void End() {
            TaskCount--;
            OnPropertyChanged("Value");
            if(LoadingChanged != null) LoadingChanged();
        }

        /// <summary>
        /// Cancels any running tasks.
        /// </summary>
        public void Cancel() {
            if(TokenSource == null)
                throw new ApplicationException("No cancellation token source found.");

            TokenSource.Cancel();
            TokenSource = new CancellationTokenSource();
            End();
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            var handler = PropertyChanged;
            if(handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
