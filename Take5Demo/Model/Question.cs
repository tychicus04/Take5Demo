using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Take5Demo.Model
{
    public class Question : INotifyPropertyChanged
    {
        private string _answer;
        private bool _hasSignature;
        private string _signatureImageSource;
        private bool _isAnswered;

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

        [JsonIgnore] 
        public string Answer
        {
            get => _answer;
            set
            {
                if (_answer != value)
                {
                    _answer = value;

                    if (ResponseType == "Signature" || Name?.Contains("Signature") == true)
                    {
                        SignatureImageSource = value;
                    }

                    IsAnswered = !string.IsNullOrEmpty(value);
                    OnPropertyChanged();
                }
            }
        }

        public int Index { get; set; }

        [JsonIgnore]
        public bool IsAnswered
        {
            get => _isAnswered;
            set
            {
                if (_isAnswered != value)
                {
                    _isAnswered = value;
                    OnPropertyChanged();
                }
            }
        }

        [JsonIgnore]
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

        [JsonIgnore]
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}