using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Take5Demo.Model;

namespace Take5Demo.ViewModel
{
    public class StepItem : INotifyPropertyChanged
    {
        #region Fields
        private string _stepId;
        private int _stepNumber;
        private string _title;
        private bool _isSelected;
        private bool _hasNextStep;
        private string _subTitle;
        private bool _isCurrentStep;
        private ObservableCollection<QuestionGroup> _questionGroups { get; set; }
        #endregion

        public StepItem()
        {
            QuestionGroups = new ObservableCollection<QuestionGroup>();
        }
        public StepItem(string stepId, int stepNumber, string title, bool isSelected, bool hasNextStep, string subTitle, bool isCurrentStep)
        {
            _stepId = stepId;
            _stepNumber = stepNumber;
            _title = title;
            _isSelected = isSelected;
            _hasNextStep = hasNextStep;
            _subTitle = subTitle;
            _isCurrentStep = isCurrentStep;
            QuestionGroups = new ObservableCollection<QuestionGroup>();
        }

        #region Properties
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public string StepId
        {
            get => _stepId;
            set
            {
                if (_stepId != value)
                {
                    _stepId = value;
                    OnPropertyChanged();
                }
            }
        }

        public int StepNumber
        {
            get => _stepNumber;
            set
            {
                if (_stepNumber != value)
                {
                    _stepNumber = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool HasNextStep
        {
            get => _hasNextStep;
            set
            {
                if (_hasNextStep != value)
                {
                    _hasNextStep = value;
                    OnPropertyChanged();
                }
            }
        }

        public string SubTitle
        {
            get => _subTitle;
            set
            {
                if(_subTitle != value)
                {
                    _subTitle = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool IsCurrentStep
        {
            get => _isCurrentStep;
            set
            {
                if (_isCurrentStep != value)
                {
                    _isCurrentStep = value;
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<QuestionGroup> QuestionGroups
        {
            get => _questionGroups;
            set
            {
                if (_questionGroups != value)
                {
                    _questionGroups = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion
    }
}
