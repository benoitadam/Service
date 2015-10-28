using SimpleService.Config;
using System;
using System.Threading;

namespace DotAdam.Service
{
    /// <summary>
    /// Timer with one call to update
    /// </summary>
    public class BusyTimer
    {
        private readonly Timer _timer;
        private Action onTicks;
        private Action<Exception> onError;
        private DateTime busyDate;
        private BusyTimerConfig config;
        private TimeSpan? period;

        /// <summary>
        /// If the timer is busy (update call but not return)
        /// </summary>
        public virtual bool IsBusy
        {
            get { return busyDate != DateTime.MinValue && (DateTime.Now - busyDate) < config.BusyRelease; }
            set { busyDate = value ? DateTime.Now : DateTime.MinValue; }
        }

        /// <summary>
        /// Init new timer with default config
        /// </summary>
        /// <param name="config"></param>
        /// <param name="onTicks"></param>
        /// <param name="onError"></param>
        public BusyTimer(Action onTicks, Action<Exception> onError = null)
            : this(new BusyTimerConfig(), onTicks, onError)
        {
        }

        /// <summary>
        /// Init new timer
        /// </summary>
        /// <param name="config"></param>
        /// <param name="onUpdate"></param>
        /// <param name="onError"></param>
        public BusyTimer(BusyTimerConfig config, Action onTicks, Action<Exception> onError = null)
        {
            _timer = new Timer(OnTicks);
            this.onTicks = onTicks;
            this.onError = onError;
            this.config = config;
        }

        /// <summary>
        /// Start the timer
        /// </summary>
        /// <returns></returns>
        public virtual BusyTimer Start()
        {
            _timer.Change(config.DueTime, config.Period);
            period = config.Period;
            return this;
        }

        /// <summary>
        /// Stop the timer
        /// </summary>
        /// <returns></returns>
        public virtual BusyTimer Stop()
        {
            period = null;
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            return this;
        }

        /// <summary>
        /// On timer callback
        /// </summary>
        /// <param name="state"></param>
        protected virtual void OnTicks(object state)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                onTicks();
            }
            catch (Exception ex)
            {
                if (onError != null)
                {
                    try
                    {
                        onError(ex);
                    }
                    catch { }
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Change the configuration of timer
        /// </summary>
        /// <param name="config"></param>
        public virtual BusyTimer Config(BusyTimerConfig config)
        {
            this.config = config;
            // If timer is started and period change
            if (period != null && config.Period != period)
            {
                _timer.Change(config.DueTime, config.Period);
                period = config.Period;
            }
            return this;
        }

        /// <summary>
        /// Timer can observe config file change
        /// </summary>
        /// <param name="watcher"></param>
        public virtual BusyTimer Watcher<T>(ConfigWatcher<T> watcher)
        {
            watcher.ConfigChanged += Watcher_ConfigChanged;
            return this;
        }

        /// <summary>
        /// On config change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Watcher_ConfigChanged(object sender, EventArgs e)
        {
            Config(config);
        }
    }
}
