using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Input;
using Take5Demo.Helper;
using Take5Demo.Model;

namespace Take5Demo.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Fields
        private int _currentStepNumber = 1;
        private ObservableCollection<StepItem> _steps = new();
        private ObservableCollection<Visitor> _visitors = new();
        private StepItem _selectedStep;
        private ConfigModel _config;
        private bool _isExpanderOpen = true;
        private string _currentStepTitle = "Step 1";
        private string _continueButtonText = "CONTINUE";
        private bool _canGoBack;
        private bool _canGoForward;
        private string _validationMessage = "Please answer all questions before proceeding.";
        private bool _isNavigating = false;

        private Dictionary<int, Dictionary<string, bool>> _stepValidationStatus = new ();

        private bool _permitAnswered;
        private bool _environmentalAnswered;
        private bool _siteRiskAnswered;
        private bool _isSiteRiskPickerAnswered;
        private bool _SOPTaskAnswered;

        private bool _isEvacuationPointAnswered;
        private bool _isLocationPickerAnswered;
        private bool _isFacilityNameAnswered;
        private bool _isSiteInfoSignatureCompleted;
        private bool _isFieldManagerPickerAnswered;
        private bool _isWorkTypePickerAnswered;
        private bool _isVisitorNameAnswered;
        private bool _isVisitorCompanyNameAnswered;
        private bool _isVisitReasonPickerAnswered;
        private bool _isSiteVisitorSignatureCompleted;
        private bool _isLeavingSiteSignatureCompleted;

        public delegate Task ShowPopupDelegate(string message);
        public ShowPopupDelegate ShowPopupRequest { get; set; }

        #endregion

        #region Commands
        public ICommand GoToNextStepCommand { get; }
        public ICommand GoToPreviousStepCommand { get; }
        public ICommand StepSelectedCommand { get; }
        public ICommand ToggleExpanderCommand { get; }
        public ICommand SubmitCommand { get; }
        public ICommand AddVisitorCommand { get; }
        public ICommand RemoveVisitorCommand { get; }

        #endregion

        public MainViewModel()
        {
            GoToNextStepCommand = new Command(async () => await GoToNextStep());
            GoToPreviousStepCommand = new Command(async () => await GoToPreviousStep());
            StepSelectedCommand = new Command<StepItem>(OnStepSelected);
            ToggleExpanderCommand = new Command(ToggleExpander);
            SubmitCommand = new Command(Submit);
            AddVisitorCommand = new Command(AddVisitorWithQuestions);
            RemoveVisitorCommand = new Command<Visitor>(RemoveVisitorWithQuestions);
            Task.Run(() => InitializeAsync());
            
        }

        #region Properties
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public ObservableCollection<StepItem> Steps
        {
            get => _steps;
            set => SetProperty(ref _steps, value);
        }

        public ObservableCollection<Visitor> Visitors
        {
            get => _visitors;
            set => SetProperty(ref _visitors, value);
        }

        public StepItem SelectedStep
        {
            get => _selectedStep;
            set
            {
                if (_selectedStep != value)
                {
                    _selectedStep = value;
                    if (_selectedStep != null)
                    {
                        foreach (var step in Steps)
                        {
                            step.IsSelected = step == SelectedStep;
                            //step.IsCurrentStep = step == SelectedStep;
                        }
                        CurrentStepNumber = SelectedStep?.StepNumber ?? 1;
                        CurrentStepTitle = SelectedStep?.SubTitle ?? $"Step {CurrentStepNumber}";
                        CanGoBack = CurrentStepNumber > 1;
                        CanGoForward = SelectedStep?.HasNextStep ?? false;
                        UpdateContinueButtonText();
                    }
                    OnPropertyChanged();
                }
            }
        }

        public int CurrentStepNumber
        {
            get => _currentStepNumber;
            set
            {
                if (_currentStepNumber != value)
                {
                    _currentStepNumber = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsStepVisible));
                }
            }
        }

        public bool CanGoBack
        {
            get => _canGoBack;
            set => SetProperty(ref _canGoBack, value);
        }

        public bool CanGoForward
        {
            get => _canGoForward;
            set => SetProperty(ref _canGoForward, value);
        }

        public bool IsExpanderOpen
        {
            get => _isExpanderOpen;
            set => SetProperty(ref _isExpanderOpen, value);
        }

        public string CurrentStepTitle
        {
            get => _currentStepTitle;
            set => SetProperty(ref _currentStepTitle, value);
        }

        public bool PermitAnswered
        {
            get => _permitAnswered;
            set 
            { 
                if(SetProperty(ref _permitAnswered, value)) UpdateStepValidation(1, "Permit", value);
            }
        }

        public bool EnvironmentalAnswered
        {
            get => _environmentalAnswered;
            set
            {
                if (SetProperty(ref _environmentalAnswered, value)) UpdateStepValidation(1, "Environmental", value);
            }
        }

        public bool SiteRiskAnswered
        {
            get => _siteRiskAnswered;
            set
            {
                if (SetProperty(ref _siteRiskAnswered, value)) UpdateStepValidation(1, "SiteRisk", value);
            }
        }

        public bool IsSiteRiskPickerAnswered
        {
            get => _isSiteRiskPickerAnswered;
            set
            {
                if (SetProperty(ref _isSiteRiskPickerAnswered, value)) UpdateStepValidation(1, "SiteRiskPicker", value);
            }
        }

        public bool SOPTaskAnswered
        {
            get => _SOPTaskAnswered;
            set
            {
                if (SetProperty(ref _SOPTaskAnswered, value)) UpdateStepValidation(1, "SOPTask", value);
            }
        }

        public bool IsEvacuationPointAnswered
        {
            get => _isEvacuationPointAnswered;
            set
            {
                if (SetProperty(ref _isEvacuationPointAnswered, value)) UpdateStepValidation(2, "EvacuationPoint", value);
            }
        }

        public bool IsLocationPickerAnswered
        {
            get => _isLocationPickerAnswered;
            set
            {
                if(SetProperty(ref _isLocationPickerAnswered, value)) UpdateStepValidation(2, "LocationPicker", value);
            }
        }

        public bool IsFacilityNameAnswered
        {
            get => _isFacilityNameAnswered;
            set
            {
                if (SetProperty(ref _isFacilityNameAnswered, value)) UpdateStepValidation(2, "FacilityName", value);
            }
        }

        public bool IsSiteInfoSignatureCompleted
        {
            get => _isSiteInfoSignatureCompleted;
            set
            {
                if (SetProperty(ref _isSiteInfoSignatureCompleted, value)) UpdateStepValidation(2, "SiteInfoSignature", value);
            }
        }

        public bool IsFieldManagerPickerAnswered
        {
            get => _isFieldManagerPickerAnswered;
            set
            {
                if (SetProperty(ref _isFieldManagerPickerAnswered, value)) UpdateStepValidation(3, "FieldManagerPicker", value);
            }
        }

        public bool IsWorkTypePickerAnswered
        {
            get => _isWorkTypePickerAnswered;
            set
            {
                if (SetProperty(ref _isWorkTypePickerAnswered, value)) UpdateStepValidation(3, "WorkTypePicker", value);
            }
        }

        public bool IsVisitorNameAnswered
        {
            get => _isVisitorNameAnswered;
            set
            {
                if (SetProperty(ref _isVisitorNameAnswered, value)) UpdateStepValidation(4, "VisitorName", value);
            }
        }

        public bool IsVisitorCompanyNameAnswered
        {
            get => _isVisitorCompanyNameAnswered;
            set
            {
                if (SetProperty(ref _isVisitorCompanyNameAnswered, value)) UpdateStepValidation(4, "VisitorCompanyName", value);
            }
        }

        public bool IsVisitReasonPickerAnswered
        {
            get => _isVisitReasonPickerAnswered;
            set
            {
                if (SetProperty(ref _isVisitReasonPickerAnswered, value)) UpdateStepValidation(4, "VisitReasonPicker", value);
            }
        }

        public bool IsSiteVisitorSignatureCompleted
        {
            get => _isSiteVisitorSignatureCompleted;
            set
            {
                if (SetProperty(ref _isSiteVisitorSignatureCompleted, value)) UpdateStepValidation(4, "SiteVisitorSignature", value);
            }
        }

        public bool IsLeavingSiteSignatureCompleted
        {
            get => _isLeavingSiteSignatureCompleted;
            set
            {
                if (SetProperty(ref _isLeavingSiteSignatureCompleted, value)) UpdateStepValidation(5, "LeavingSiteSignature", value);
            }
        }
        public string ContinueButtonText
        {
            get => _continueButtonText;
            set => SetProperty(ref _continueButtonText, value);
        }
        #endregion

        #region Methods
        private async void InitializeAsync()
        {
            try
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync("config.json");
                using var reader = new StreamReader(stream);
                var json = await reader.ReadToEndAsync();

                _config = JsonSerializer.Deserialize<ConfigModel>(
                    json, 
                    new JsonSerializerOptions { 
                        PropertyNameCaseInsensitive = true 
                    });
                InitializeSteps();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading configuration: {ex.Message}");
                LoadDefaultSteps();
            }
        }
        public bool IsStepVisible(int stepNumber)
        {
            return stepNumber == CurrentStepNumber;
        }
        public int TotalSteps => Steps?.Count ?? 0;
        private void InitializeSteps()
        {
            Steps.Clear();
            _stepValidationStatus.Clear();

            if (_config?.JobTab?.Steppers?.FirstOrDefault()?.SubSteppers is { } subSteppers)
            {
                var dataFeatures = _config.DataLists;

                for (int i = 0; i < subSteppers.Count; i++)
                {
                    var subStepper = subSteppers[i];
                    int stepNumber = i + 1;
                    var dataFeature = dataFeatures.FirstOrDefault(df => df.FeatureKey == subStepper.FeatureKey);

                    var step = new StepItem
                    {
                        StepId = subStepper.FeatureKey,
                        StepNumber = stepNumber,
                        Title = subStepper.Title,
                        IsSelected = i == 0,
                        IsCurrentStep = i == 0,
                        HasNextStep = i < subSteppers.Count - 1,
                        SubTitle = subStepper.SubTitle,
                        QuestionGroups = CreateQuestionGroups(dataFeature?.QuestionSet?.QuestionGroups)
                    };

                    Steps.Add(step);
                    InitializeStepValidation(stepNumber, subStepper.FeatureKey);
                }
            }

            if (Steps.Count == 0)
            {
                LoadDefaultSteps();
            }
            else
            {
                SelectedStep = Steps.FirstOrDefault(s => s.IsSelected);
                OnPropertyChanged(nameof(TotalSteps));
            }
        }

        private ObservableCollection<QuestionGroup> CreateQuestionGroups(IEnumerable<QuestionGroup> questionGroups)
        {
            if (questionGroups == null) return new ObservableCollection<QuestionGroup>();

            var result = new ObservableCollection<QuestionGroup>();
            foreach (var group in questionGroups)
            {
                var newGroup = new QuestionGroup
                {
                    Name = group.Name,
                    Description = group.Description,
                    Questions = group.Questions?.Select(q => new Question
                    {
                        Name = q.Name,
                        Description = q.Description,
                        ResponseType = q.ResponseType,
                        IsMandatory = q.IsMandatory,
                        Value = q.Value
                    }).ToList() ?? new List<Question>()
                };
                result.Add(newGroup);
            }
            return result;
        }

        private void FindAndUpdateVisitorForQuestion(Question question)
        {
            foreach (var visitor in Visitors)
            {
                if (visitor.Questions != null && visitor.Questions.Contains(question))
                {
                    VisitorQuestionFactory.SyncQuestionsToVisitorData(visitor);
                    ValidateVisitors();
                    break;
                }
            }
        }

        private void AddVisitorWithQuestions()
        {
            var newVisitor = VisitorQuestionFactory.CreateVisitorWithQuestions(_config);
            Visitors.Add(newVisitor);
            ValidateVisitors();
        }

        private void RemoveVisitorWithQuestions(Visitor visitor)
        {
            if (visitor != null && Visitors.Contains(visitor))
            {
                Visitors.Remove(visitor);
                ValidateVisitors();
            }
        }

        public void ValidateVisitors()
        {
            bool hasCompleteVisitor = false;

            if (Visitors.Count > 0)
            {
                hasCompleteVisitor = Visitors.Any(visitor =>
                    !string.IsNullOrWhiteSpace(visitor.VisitorName) &&
                    !string.IsNullOrWhiteSpace(visitor.VisitorCompany) &&
                    !string.IsNullOrWhiteSpace(visitor.VisitReason) &&
                    visitor.HasSignature &&
                    visitor.AreRequiredQuestionsAnswered);
            }

            IsVisitorNameAnswered = hasCompleteVisitor;
            IsVisitorCompanyNameAnswered = hasCompleteVisitor;
            IsVisitReasonPickerAnswered = hasCompleteVisitor;
            IsSiteVisitorSignatureCompleted = hasCompleteVisitor;
        }

        private bool AreVisitorsValid()
        {
            if (Visitors == null || Visitors.Count == 0)
                return false;

            return Visitors.All(visitor =>
                !string.IsNullOrWhiteSpace(visitor.VisitorName) &&
                !string.IsNullOrWhiteSpace(visitor.VisitorCompany) &&
                !string.IsNullOrWhiteSpace(visitor.VisitReason) &&
                visitor.HasSignature);
        }
        private void LoadDefaultSteps()
        {
            Steps.Clear();
            _stepValidationStatus.Clear();

            Steps.Add(new StepItem
            {
                StepId = "Step1",
                StepNumber = 1,
                Title = "Step 1",
                SubTitle = "Precursors",
                IsSelected = true,
                HasNextStep = true
            });

            _stepValidationStatus[1] = new Dictionary<string, bool>
            {
                { "Permit", false },
            };

            SelectedStep = Steps[0];
            CurrentStepNumber = 1;
            OnPropertyChanged(nameof(TotalSteps));
        }

        private void UpdateStepValidation(int stepNumber, string field, bool isValid)
        {
            if (!_stepValidationStatus.ContainsKey(stepNumber))
            {
                _stepValidationStatus[stepNumber] = new Dictionary<string, bool>();
            }

            _stepValidationStatus[stepNumber][field] = isValid;
        }
        private void ToggleExpander()
        {
            IsExpanderOpen = !IsExpanderOpen;
        }

        private async void OnStepSelected(StepItem step)
        {
            if (step != null && step != SelectedStep)
            {
                await NavigateToStep(step.StepNumber);
            }
        }

        private bool IsPreviousStepsValid(int targetStepNumber)
        {
            for (int stepNum = 1; stepNum < targetStepNumber; stepNum++)
            {
                if (!IsStepValid(stepNum))
                    return false;
            }
            return true;
        }

        private bool IsStepValid(int stepNumber)
        {
            if (stepNumber <= 0 || stepNumber > Steps.Count ||
                !_stepValidationStatus.TryGetValue(stepNumber, out var validationFields))
                return false;

            if (validationFields.Count == 0)
                return true;

            var step = Steps[stepNumber - 1];

            if (step.StepId == "site-crew")
                return AreVisitorsValid();

            return step.QuestionGroups?.SelectMany(group => group.Questions)
                                     .Where(q => q.IsMandatory)
                                     .All(q => !string.IsNullOrWhiteSpace(q.Answer)) ?? false;
        }

        private async Task NavigateToStep(int stepNumber)
        {
            _isNavigating = true;
            try
            {
                if (stepNumber > CurrentStepNumber && !IsPreviousStepsValid(stepNumber))
                {
                    if (ShowPopupRequest != null)
                    {
                        await ShowPopupRequest(_validationMessage);
                    }
                    return;
                }

                if (Steps[stepNumber - 1].StepId == "site-crew" && (Visitors?.Count ?? 0) == 0)
                {
                    Visitors = new ObservableCollection<Visitor>();
                    AddVisitorWithQuestions();
                }

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    var step = Steps.FirstOrDefault(s => s.StepNumber == stepNumber);
                    if (step != null)
                    {
                        SelectedStep = step;
                    }
                });
            }
            finally
            {
                _isNavigating = false;
            }



        }

        private async Task GoToNextStep()
        {
            if (SelectedStep?.HasNextStep == true)
            {
                await NavigateToStep(CurrentStepNumber + 1);
            }
            else
            {
                Submit();
            }
        }
        private async Task GoToPreviousStep() =>
            await NavigateToStep(CurrentStepNumber - 1);

        private async void Submit()
        {
            if (!IsStepValid(CurrentStepNumber))
            {
                if (ShowPopupRequest != null)
                {
                    await ShowPopupRequest(_validationMessage);
                }
                return;
            }
            try
            {
                
                await Application.Current.MainPage.DisplayAlert("Success", "Your form has been submitted successfully!", "OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting file: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "An error occurred while submitting the form.", "OK");
            }
        }

        public void HandleRadioButtonCheckedChanged(object sender, CheckedChangedEventArgs e, Question question)
        {
            if (_isNavigating) return;
            if (sender is RadioButton radioButton)
            {
                HandleQuestionAnswered(question, radioButton.Content?.ToString());
            }

            FindAndUpdateVisitorForQuestion(question);
        }

        public void HandlePickerSelectedIndexChanged(object sender, EventArgs e, Question question)
        {
            if (sender is Picker picker && picker.SelectedIndex >= 0)
            {
                HandleQuestionAnswered(question, picker.SelectedItem?.ToString());
            }

            FindAndUpdateVisitorForQuestion(question);
        }

        public void HandleEntryTextChanged(object sender, TextChangedEventArgs e, Question question)
        {
            if (sender is Entry entry)
            {
                HandleQuestionAnswered(question, entry.Text);
            }

            FindAndUpdateVisitorForQuestion(question);
        }

        public void HandleSignatureCompleted(object sender, EventArgs e, Question question, string base64String)
        {
            if (string.IsNullOrEmpty(base64String))
            {
                return;
            }
            HandleQuestionAnswered(question, base64String);
            question.SignatureImageSource = base64String;
            question.HasSignature = true;
            FindAndUpdateVisitorForQuestion(question);
        }
        public void HandleSignatureCleared(object sender, EventArgs e, Question question)
        {
            HandleQuestionAnswered(question, string.Empty);
            question.SignatureImageSource = null;
            question.HasSignature = false;
        }
        public void HandleQuestionAnswered(Question question, string answer)
        {
            if (question == null) return;

            question.Answer = answer;

            //foreach (var step in Steps)
            //{
            //    foreach (var group in step.QuestionGroups)
            //    {
            //        foreach (var q in group.Questions)
            //        {
            //            if (q.Name == question.Name)
            //            {
            //                q.Answer = answer;
            //                if (q.ResponseType == "Signature")
            //                {
            //                    q.HasSignature = question.HasSignature;
            //                    q.SignatureImageSource = question.SignatureImageSource;
            //                }
            //            }
            //        }
            //    }
            //}
            SelectedStep.QuestionGroups.SelectMany(group => group.Questions)
                .Where(q => q.Name == question.Name)
                .ToList()
                .ForEach(q =>
                {
                    q.Answer = answer;
                    if (q.ResponseType == "Signature")
                    {
                        q.HasSignature = question.HasSignature;
                        q.SignatureImageSource = question.SignatureImageSource;
                    }
                });

            bool isAnswered = !string.IsNullOrEmpty(answer);

            switch (question.Name)
            {
                case "Is there are permit required?":
                    PermitAnswered = isAnswered;
                    break;
                case "Is there an environmental risk onsite?":
                    EnvironmentalAnswered = isAnswered;
                    break;
                case "Is there a site risk with the task i.e. Working at Height <2m?":
                    SiteRiskAnswered = isAnswered;
                    break;
                case "Assess the site risks for people/animals/environment?":
                    IsSiteRiskPickerAnswered = isAnswered;
                    break;
                case "Is there an SOP for the task?":
                    SOPTaskAnswered = isAnswered;
                    break;
                case "Evacuation Point":
                    IsEvacuationPointAnswered = isAnswered;
                    break;
                case "Location":
                    IsLocationPickerAnswered = isAnswered;
                    break;
                case "Facility Name":
                    IsFacilityNameAnswered = isAnswered;
                    break;
                case "Site Information Signature":
                    IsSiteInfoSignatureCompleted = isAnswered;
                    break;
                case "Field Manager":
                    IsFieldManagerPickerAnswered = isAnswered;
                    break;
                case "Work Type":
                    IsWorkTypePickerAnswered = isAnswered;
                    break;
                case "Visitor Name":
                    IsVisitorNameAnswered = isAnswered;
                    break;
                case "Visitor Company":
                    IsVisitorCompanyNameAnswered = isAnswered;
                    break;
                case "Reason for Visit":
                    IsVisitReasonPickerAnswered = isAnswered;
                    break;
                case "Site Visitor Signature":
                    IsSiteVisitorSignatureCompleted = isAnswered;
                    break;
                case "Leaving Site Signature":
                    IsLeavingSiteSignatureCompleted = isAnswered;
                    break;
            }
        }

        private void InitializeStepValidation(int stepNumber, string stepId)
        {
            _stepValidationStatus[stepNumber] = new Dictionary<string, bool>();

            switch (stepNumber)
            {
                case 1:
                    _stepValidationStatus[stepNumber]["Permit"] = PermitAnswered;
                    _stepValidationStatus[stepNumber]["Environmental"] = EnvironmentalAnswered;
                    _stepValidationStatus[stepNumber]["SiteRisk"] = SiteRiskAnswered;
                    _stepValidationStatus[stepNumber]["SiteRiskPicker"] = IsSiteRiskPickerAnswered;
                    _stepValidationStatus[stepNumber]["SOPTask"] = SOPTaskAnswered;
                    break;

                case 2:
                    _stepValidationStatus[stepNumber]["EvacuationPoint"] = IsEvacuationPointAnswered;
                    _stepValidationStatus[stepNumber]["LocationPicker"] = IsLocationPickerAnswered;
                    _stepValidationStatus[stepNumber]["FacilityName"] = IsFacilityNameAnswered;
                    _stepValidationStatus[stepNumber]["SiteInfoSignature"] = IsSiteInfoSignatureCompleted;
                    break;

                case 3:
                    _stepValidationStatus[stepNumber]["FieldManagerPicker"] = IsFieldManagerPickerAnswered;
                    _stepValidationStatus[stepNumber]["WorkTypePicker"] = IsWorkTypePickerAnswered;
                    break;

                case 4:
                    _stepValidationStatus[stepNumber]["VisitorName"] = IsVisitorNameAnswered;
                    _stepValidationStatus[stepNumber]["VisitorCompanyName"] = IsVisitorCompanyNameAnswered;
                    _stepValidationStatus[stepNumber]["VisitReasonPicker"] = IsVisitReasonPickerAnswered;
                    _stepValidationStatus[stepNumber]["SiteVisitorSignature"] = IsSiteVisitorSignatureCompleted;
                    break;
                case 5:
                    _stepValidationStatus[stepNumber]["LeavingSiteSignature"] = IsLeavingSiteSignatureCompleted;
                    break;
                default:
                    break;
            }
        }

        private void UpdateContinueButtonText()
        {
            ContinueButtonText = (CurrentStepNumber == Steps?.Count) ? "SUBMIT" : "CONTINUE";
        }

        private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        #endregion
    }
}