using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Take5Demo.Model
{
    public class Visitor : INotifyPropertyChanged
    {
        private string _visitorName;
        private string _visitorCompany;
        private string _visitReason;
        private bool _isExpanded;
        private bool _hasSignature;
        private string _signatureBase64;
        private string _signaturePadId;
        private ObservableCollection<Question> _questions;

        public string VisitorName
        {
            get => _visitorName;
            set
            {
                if (_visitorName != value)
                {
                    _visitorName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string VisitorCompany
        {
            get => _visitorCompany;
            set
            {
                if (_visitorCompany != value)
                {
                    _visitorCompany = value;
                    OnPropertyChanged();
                }
            }
        }

        public string VisitReason
        {
            get => _visitReason;
            set
            {
                if (_visitReason != value)
                {
                    _visitReason = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool HasSignature
        {
            get => _hasSignature;
            set
            {
                if (_hasSignature != value)
                {
                    _hasSignature = value;
                    OnPropertyChanged();
                }
            }
        }

        public string SignatureBase64
        {
            get => _signatureBase64;
            set
            {
                if (_signatureBase64 != value)
                {
                    _signatureBase64 = value;
                    HasSignature = !string.IsNullOrEmpty(value);
                    OnPropertyChanged();
                }
            }
        }

        public string SignaturePadId
        {
            get => _signaturePadId ?? (_signaturePadId = Guid.NewGuid().ToString());
            set
            {
                if (_signaturePadId != value)
                {
                    _signaturePadId = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<Question> Questions
        {
            get => _questions;
            set
            {
                if (_questions != value)
                {
                    _questions = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool AreRequiredQuestionsAnswered
        {
            get
            {
                if (Questions == null) return true;
                return !Questions.Any(q => q.IsMandatory && string.IsNullOrWhiteSpace(q.Answer));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
