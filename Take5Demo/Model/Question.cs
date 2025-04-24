using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Take5Demo.Model
{
    public class Question
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("responseType")]
        public string ResponseType { get; set; }
        [JsonPropertyName("isMandatory")]
        public int IsMandatoryValue { get; set; }
        [JsonIgnore]
        public bool IsMandatory
        {
            get => IsMandatoryValue == 1;
            set => IsMandatoryValue = value ? 1 : 0;
        }
        [JsonPropertyName("value")]
        public List<string> Value { get; set; }
        [JsonPropertyName("logic")]
        public string Logic { get; set; }
        public string Answer { get; set; }
        public int Index { get; set; }
    }

    public class QuestionViewModel : Question, INotifyPropertyChanged
    {
        private bool _hasSignature;
        private string _signatureImageSource;
        private string _signatureStatus;
        public QuestionGroup ParentGroup { get; set; }
        public bool IsAnswered { get; set; }

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

        public string SignatureImageSource
        {
            get => _signatureImageSource;
            set
            {
                if (_signatureImageSource != value)
                {
                    _signatureImageSource = value;
                    HasSignature = !string.IsNullOrEmpty(value);
                    OnPropertyChanged();
                }
            }
        }

        public string SignatureStatus
        {
            get => _signatureStatus;
            set
            {
                if (_signatureStatus != value)
                {
                    _signatureStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        public new string Answer
        {
            get => base.Answer;
            set
            {
                base.Answer = value;

                if (ResponseType == "Signature" || Name?.Contains("Signature") == true)
                {
                    SignatureImageSource = value;
                }

                IsAnswered = !string.IsNullOrEmpty(value);
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
