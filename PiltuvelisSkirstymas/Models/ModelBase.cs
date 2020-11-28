using Microsoft.Toolkit.Mvvm.ComponentModel;
using PiltuvelisSkirstymas.Enums;
using System.Reactive.Subjects;

namespace PiltuvelisSkirstymas.Models
{
    public class ModelBase : ObservableObject
    {
        private string _statusBarMessage;
        private MessageType _messageType;

        public string StatusBarMessage
        {
            get => _statusBarMessage;
            set => SetProperty(ref _statusBarMessage, value);
        }

        public MessageType MessageType
        {
            get => _messageType;
            set
            {
                SetProperty(ref _messageType, value);
                _messageTypeSubject.OnNext(value);
            }
        }

        private ISubject<MessageType> _messageTypeSubject = new Subject<MessageType>();

        public ISubject<MessageType> StatusBarObservable
        {
            get => _messageTypeSubject;
            set
            {
                if (value != _messageTypeSubject)
                    _messageTypeSubject = value;
            }
        }
    }
}
