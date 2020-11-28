using PiltuvelisSkirstymas.Enums;
using PiltuvelisSkirstymas.Models;
using System;
using System.Linq;
using System.Reactive.Linq;

namespace PiltuvelisSkirstymas.ViewModels
{
    public class ViewModelBase<T> where T : ModelBase, new()
    {
        public T Model { get; private set; }

        /// <summary>
        /// Specifies how long will a status bar message be shown when ShowFadingStatusBarMessage method is used
        /// </summary>
        public TimeSpan StatusBarFadeTime { get; set; }
        private IDisposable _statusBarSubscription;

        public ViewModelBase()
        {
            Model = new T();
            Initialize();
        }

        private void Initialize()
        {
            StatusBarFadeTime = TimeSpan.FromSeconds(10);
            Model.MessageType = MessageType.Empty;
            Model.StatusBarObservable
                 .Where(x => x != MessageType.Empty)
                 .Subscribe(OnStatusBarMessageChanged);
        }

        private void OnStatusBarMessageChanged(MessageType obj)
        {
            _statusBarSubscription?.Dispose();
            _statusBarSubscription = Observable
                .Timer(StatusBarFadeTime)
                .Subscribe(x =>
                {
                    Model.StatusBarMessage = string.Empty;
                    Model.MessageType = MessageType.Empty;
                });

        }

        public void ShowFadingStatusBarMessage(MessageType messageType, string message)
        {
            Model.MessageType = messageType;
            Model.StatusBarMessage = message;
        }
    }
}
