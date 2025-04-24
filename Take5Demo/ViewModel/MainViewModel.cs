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

        private ObservableCollection<StepItem> _steps;
        private ObservableCollection<Visitor> _visitors;
        private ObservableCollection<QuestionGroup> _questionGroups;
        private StepItem _selectedStep;
        private ConfigModel _config;
        private bool _isExpanderOpen = true;
        private string _currentStepTitle = "Step 1";
        private string _continueButtonText = "CONTINUE";

        private bool _canGoBack;
        private bool _canGoForward;

        private string _validationMessage = "Please answer all questions before proceeding.";

        private Dictionary<int, Dictionary<string, bool>> _stepValidationStatus = new Dictionary<int, Dictionary<string, bool>>();
        private Dictionary<string, List<QuestionGroup>> _stepQuestionCache = new Dictionary<string, List<QuestionGroup>>();
        private Dictionary<string, ObservableCollection<Visitor>> _stepVisitorCache = new Dictionary<string, ObservableCollection<Visitor>>();

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
        private string _signatureStatus;

        public delegate Task ShowPopupDelegate(string message);
        public ShowPopupDelegate ShowPopupRequest { get; set; }

        #endregion

        #region Commands
        public ICommand GoToNextStepCommand { get; }
        public ICommand GoToPreviousStepCommand { get; }
        public ICommand StepSelectedCommand { get; }
        public ICommand ToggleExpanderCommand { get; }
        public ICommand SubmitCommand { get; }
        public ICommand ValidateSiteInfoSignatureCommand { get; }
        public ICommand ValidateSiteVisitorSignatureCommand { get; }
        public ICommand ValidateLeavingSiteSignatureCommand { get; }
        public ICommand AddVisitorCommand { get; }
        public ICommand RemoveVisitorCommand { get; }

        #endregion

        public MainViewModel()
        {
            GoToNextStepCommand = new Command(async () => await GoToNextStep());
            GoToPreviousStepCommand = new Command(GoToPreviousStep);
            StepSelectedCommand = new Command<StepItem>(OnStepSelected);
            ToggleExpanderCommand = new Command(ToggleExpander);
            SubmitCommand = new Command(Submit);

            Steps = new ObservableCollection<StepItem>();

            AddVisitorCommand = new Command(AddVisitorWithQuestions);
            RemoveVisitorCommand = new Command<Visitor>(RemoveVisitorWithQuestions);
            Visitors = new ObservableCollection<Visitor>();

            Task.Run(() => LoadConfiguration());
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
            set
            {
                if (_steps != value)
                {
                    _steps = value;

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

        public ObservableCollection<Visitor> Visitors
        {
            get => _visitors;
            set
            {
                if (_visitors != value)
                {
                    _visitors = value;
                    OnPropertyChanged();
                }
            }
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
                        UpdateSelectedStep();
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
            set
            {
                if (_canGoBack != value)
                {
                    _canGoBack = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool CanGoForward
        {
            get => _canGoForward;
            set
            {
                if (_canGoForward != value)
                {
                    _canGoForward = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsExpanderOpen
        {
            get => _isExpanderOpen;
            set
            {
                if (_isExpanderOpen != value)
                {
                    _isExpanderOpen = value;
                    OnPropertyChanged();
                }
            }
        }

        public string CurrentStepTitle
        {
            get => _currentStepTitle;
            set
            {
                if (_currentStepTitle != value)
                {
                    _currentStepTitle = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool PermitAnswered
        {
            get => _permitAnswered;
            set
            {
                if (_permitAnswered != value)
                {
                    _permitAnswered = value;
                    UpdateStepValidation(1, "Permit", value);
                    OnPropertyChanged();
        
                }
            }
        }

        public bool EnvironmentalAnswered
        {
            get => _environmentalAnswered;
            set
            {
                if (_environmentalAnswered != value)
                {
                    _environmentalAnswered = value;
                    UpdateStepValidation(1, "Environmental", value);
                    OnPropertyChanged();
    
                }
            }
        }

        public bool SiteRiskAnswered
        {
            get => _siteRiskAnswered;
            set
            {
                if (_siteRiskAnswered != value)
                {
                    _siteRiskAnswered = value;
                    UpdateStepValidation(1, "SiteRisk", value);
                    OnPropertyChanged();
             
                }
            }
        }

        public bool IsSiteRiskPickerAnswered
        {
            get => _isSiteRiskPickerAnswered;
            set
            {
                if (_isSiteRiskPickerAnswered != value)
                {
                    _isSiteRiskPickerAnswered = value;
                    UpdateStepValidation(1, "SiteRiskPicker", value);
                    OnPropertyChanged();
               
                }
            }
        }

        public bool SOPTaskAnswered
        {
            get => _SOPTaskAnswered;
            set
            {
                if (_SOPTaskAnswered != value)
                {
                    _SOPTaskAnswered = value;
                    UpdateStepValidation(1, "SOPTask", value);
                    OnPropertyChanged();

                }
            }
        }

        public bool IsEvacuationPointAnswered
        {
            get => _isEvacuationPointAnswered;
            set
            {
                if (_isEvacuationPointAnswered != value)
                {
                    _isEvacuationPointAnswered = value;
                    UpdateStepValidation(2, "EvacuationPoint", value);
                    OnPropertyChanged();
            
                }
            }
        }

        public bool IsLocationPickerAnswered
        {
            get => _isLocationPickerAnswered;
            set
            {
                if (_isLocationPickerAnswered != value)
                {
                    _isLocationPickerAnswered = value;
                    UpdateStepValidation(2, "LocationPicker", value);
                    OnPropertyChanged();
            
                }
            }
        }

        public bool IsFacilityNameAnswered
        {
            get => _isFacilityNameAnswered;
            set
            {
                if (_isFacilityNameAnswered != value)
                {
                    _isFacilityNameAnswered = value;
                    UpdateStepValidation(2, "FacilityName", value);
                    OnPropertyChanged();
        
                }
            }
        }

        public bool IsSiteInfoSignatureCompleted
        {
            get => _isSiteInfoSignatureCompleted;
            set
            {
                if (_isSiteInfoSignatureCompleted != value)
                {
                    _isSiteInfoSignatureCompleted = value;
                    UpdateStepValidation(2, "SiteInfoSignature", value);
                    OnPropertyChanged();
             
                }
            }
        }

        public bool IsFieldManagerPickerAnswered
        {
            get => _isFieldManagerPickerAnswered;
            set
            {
                if (_isFieldManagerPickerAnswered != value)
                {
                    _isFieldManagerPickerAnswered = value;
                    UpdateStepValidation(3, "FieldManagerPicker", value);
                    OnPropertyChanged();
          
                }
            }
        }

        public bool IsWorkTypePickerAnswered
        {
            get => _isWorkTypePickerAnswered;
            set
            {
                if (_isWorkTypePickerAnswered != value)
                {
                    _isWorkTypePickerAnswered = value;
                    UpdateStepValidation(3, "WorkTypePicker", value);
                    OnPropertyChanged();
          
                }
            }
        }

        public bool IsVisitorNameAnswered
        {
            get => _isVisitorNameAnswered;
            set
            {
                if (_isVisitorNameAnswered != value)
                {
                    _isVisitorNameAnswered = value;
                    UpdateStepValidation(4, "VisitorName", value);
                    OnPropertyChanged();

                }
            }
        }

        public bool IsVisitorCompanyNameAnswered
        {
            get => _isVisitorCompanyNameAnswered;
            set
            {
                if (_isVisitorCompanyNameAnswered != value)
                {
                    _isVisitorCompanyNameAnswered = value;
                    UpdateStepValidation(4, "VisitorCompanyName", value);
                    OnPropertyChanged();
              
                }
            }
        }

        public bool IsVisitReasonPickerAnswered
        {
            get => _isVisitReasonPickerAnswered;
            set
            {
                if (_isVisitReasonPickerAnswered != value)
                {
                    _isVisitReasonPickerAnswered = value;
                    UpdateStepValidation(4, "VisitReasonPicker", value);
                    OnPropertyChanged();
                }
            }
        }

        public bool IsSiteVisitorSignatureCompleted
        {
            get => _isSiteVisitorSignatureCompleted;
            set
            {
                if (_isSiteVisitorSignatureCompleted != value)
                {
                    _isSiteVisitorSignatureCompleted = value;
                    UpdateStepValidation(4, "SiteVisitorSignature", value);
                    OnPropertyChanged();
                }
            }
        }

        public bool IsLeavingSiteSignatureCompleted
        {
            get => _isLeavingSiteSignatureCompleted;
            set
            {
                if (_isLeavingSiteSignatureCompleted != value)
                {
                    _isLeavingSiteSignatureCompleted = value;
                    UpdateStepValidation(5, "LeavingSiteSignature", value);
                    OnPropertyChanged();
                }
            }
        }
        public string SignatureStatus
        {
            get => _signatureStatus;
            set
            {
                _signatureStatus = value;
                OnPropertyChanged(nameof(SignatureStatus));
            }
        }
        public string ContinueButtonText
        {
            get => _continueButtonText;
            set
            {
                if (_continueButtonText != value)
                {
                    _continueButtonText = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Methods
        public bool IsStepVisible(int stepNumber)
        {
            return stepNumber == CurrentStepNumber;
        }
        public int TotalSteps => Steps?.Count ?? 0;
        private async void LoadConfiguration()
        {
            try
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync("config.json");
                using var reader = new StreamReader(stream);
                var json = await reader.ReadToEndAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                _config = JsonSerializer.Deserialize<ConfigModel>(json, options);

                if (_config != null)
                {
                    InitializeSteps();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading configuration: {ex.Message}");
                LoadDefaultSteps();
            }
        }

        private void InitializeSteps()
        {
            Steps.Clear();
            _stepValidationStatus.Clear();

            if (_config?.JobTab?.Steppers != null && _config.JobTab.Steppers.Count > 0)
            {
                var subSteppers = _config.JobTab.Steppers[0].SubSteppers;

                if (subSteppers != null)
                {
                    for (int i = 0; i < subSteppers.Count; i++)
                    {
                        var subStepper = subSteppers[i];
                        int stepNumber = i + 1;
                        Steps.Add(new StepItem
                        {
                            StepId = subStepper.FeatureKey,
                            StepNumber = i + 1,
                            Title = subStepper.Title,
                            IsSelected = i == 0,
                            IsCurrentStep = i == 0,
                            HasNextStep = i < subSteppers.Count - 1,
                            SubTitle = subStepper.SubTitle,
                            QuestionGroups = new ObservableCollection<QuestionGroup>()

                        });

                        InitializeStepValidation(stepNumber, subStepper.FeatureKey);
                    }
                }
            }

            if (Steps.Count == 0)
            {
                LoadDefaultSteps();
            }

            if (Steps.Count > 0)
            {
                SelectedStep = Steps.FirstOrDefault(s => s.IsSelected);
                CurrentStepNumber = SelectedStep?.StepNumber ?? 1;
                OnPropertyChanged(nameof(TotalSteps));
                LoadQuestionsForCurrentStep();
            }
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
                visitor.HasSignature &&
                visitor.AreRequiredQuestionsAnswered);
        }
        private void LoadQuestionsForCurrentStep()
        {
            try
            {
                if (SelectedStep == null) return;

                string stepId = SelectedStep.StepId;

                //if (_stepQuestionCache.TryGetValue(stepId, out var cachedQuestions) && cachedQuestions != null)
                //{
                //    QuestionGroups = new ObservableCollection<QuestionGroup>(cachedQuestions);
                //    UpdateStepQuestionGroups(stepId, cachedQuestions);

                //    if (stepId == "site-crew" && _stepVisitorCache.TryGetValue(stepId, out var cachedVisitors))
                //    {
                //        Visitors = cachedVisitors;
                //    }

                //    UpdateStepStates();
                //    UpdateContinueButtonText();
                //    return;
                //}

                var takeFeature = _config?.DataLists?.FirstOrDefault(d => d.FeatureKey == stepId);
                var questionGroups = takeFeature?.QuestionSet?.QuestionGroups ?? new List<QuestionGroup>();

                if (stepId == "site-crew")
                {
                    Visitors ??= new ObservableCollection<Visitor>();
                    if (Visitors.Count == 0) AddVisitorWithQuestions();
                    _stepVisitorCache[stepId] = Visitors;
                }

                QuestionGroups = new ObservableCollection<QuestionGroup>(questionGroups);
                _stepQuestionCache[stepId] = new List<QuestionGroup>(questionGroups);

                UpdateStepQuestionGroups(stepId, questionGroups);
                ValidateVisitors();
                UpdateContinueButtonText();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading questions: {ex.Message}");
            }
        }

        private void UpdateStepQuestionGroups(string stepId, IEnumerable<QuestionGroup> questionGroups)
        {
            foreach (var step in Steps)
            {
                if (step.StepId == stepId)
                {
                    step.QuestionGroups = new ObservableCollection<QuestionGroup>(questionGroups);
                }
            }
        }

        private void UpdateStepStates()
        {
            foreach (var step in Steps)
            {
                step.IsCurrentStep = step.StepNumber == CurrentStepNumber;
            }
        }

        public void HandleQuestionAnswered(Question question, string answer)
        {
            if (question == null) return;

            question.Answer = answer;
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

        private void UpdateSelectedStep()
        {
            foreach (var step in Steps)
            {
                step.IsSelected = step == SelectedStep;
                step.IsCurrentStep = step == SelectedStep;
            }

            CurrentStepNumber = SelectedStep?.StepNumber ?? 1;
            CurrentStepTitle = SelectedStep?.SubTitle ?? $"Step {CurrentStepNumber}";
            CanGoBack = CurrentStepNumber > 1;
            CanGoForward = SelectedStep?.HasNextStep ?? false;

            UpdateContinueButtonText();
        }

        private void UpdateStepValidation(int stepNumber, string field, bool isValid)
        {
            if (!_stepValidationStatus.ContainsKey(stepNumber))
            {
                _stepValidationStatus[stepNumber] = new Dictionary<string, bool>();
            }

            _stepValidationStatus[stepNumber][field] = isValid;
        }

        private async void OnStepSelected(StepItem step)
        {
            if (step != null && step != SelectedStep)
            {
                if (IsCurrentStepValid() || step.StepNumber < SelectedStep.StepNumber)
                {
                    if (SelectedStep != null)
                    {
                        SaveCurrentStepData();
                    }

                    SelectedStep = step;
                    LoadQuestionsForCurrentStep();
                }
                else
                {
                    if (ShowPopupRequest != null)
                    {
                        await ShowPopupRequest(_validationMessage);
                    }
                }
            }

            await Task.Delay(200).ContinueWith(_ =>
            {
                IsExpanderOpen = false;
                OnPropertyChanged(nameof(IsExpanderOpen));
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void SaveCurrentStepData()
        {
            try
            {
                if (SelectedStep == null) return;

                string stepId = SelectedStep.StepId;

                if (QuestionGroups != null && QuestionGroups.Count > 0)
                {
                    _stepQuestionCache[stepId] = new List<QuestionGroup>(QuestionGroups);
                }

                if (stepId == "site-crew" && Visitors != null)
                {
                    _stepVisitorCache[stepId] = Visitors;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving step data: {ex.Message}");
            }
        }

        private void ToggleExpander()
        {
            IsExpanderOpen = !IsExpanderOpen;
        }

        private async Task GoToNextStep()
        {
            if (SelectedStep != null)
            {
                if (!IsCurrentStepValid())
                {
                    if (ShowPopupRequest != null)
                    {
                        await ShowPopupRequest(_validationMessage);
                    }
                    return;
                }

                SaveCurrentStepData();

                if (CurrentStepNumber == Steps.Count)
                {
                    Submit();
                    return;
                }

                int nextIndex = Steps.IndexOf(SelectedStep) + 1;
                if (nextIndex < Steps.Count)
                {
                    SelectedStep = Steps[nextIndex];
                    LoadQuestionsForCurrentStep();
                }
            }
        }

        private void GoToPreviousStep()
        {
            if (SelectedStep != null && SelectedStep.StepNumber > 1)
            {
                SaveCurrentStepData();

                int previousIndex = Steps.IndexOf(SelectedStep) - 1;
                if (previousIndex >= 0)
                {
                    SelectedStep = Steps[previousIndex];
                    LoadQuestionsForCurrentStep();
                }
            }
        }

        private bool IsCurrentStepValid()
        {
            if (CurrentStepNumber <= 0 ||
                !_stepValidationStatus.TryGetValue(CurrentStepNumber, out var validationFields))
                return false;

            if (validationFields.Count == 0)
                return true;

            if (CurrentStepNumber == 4)
                return AreVisitorsValid();

            return QuestionGroups?.SelectMany(group => group.Questions)
                                 .Where(q => q.IsMandatory)
                                 .All(q => !string.IsNullOrWhiteSpace(q.Answer)) ?? false;
        }

        private async void Submit()
        {
            if (!IsCurrentStepValid())
            {
                if (ShowPopupRequest != null)
                {
                    ShowPopupRequest(_validationMessage);
                }
                return;
            }
            try
            {
                Application.Current.MainPage.DisplayAlert("Success", "Your form has been submitted successfully!", "OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting file: {ex.Message}");
                Application.Current.MainPage.DisplayAlert("Error", "An error occurred while submitting the form.", "OK");
            }
        }

        public void HandleRadioButtonCheckedChanged(object sender, CheckedChangedEventArgs e, Question question)
        {
            if (e.Value && sender is RadioButton radioButton)
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
            SignatureStatus = "Signature accepted";
            FindAndUpdateVisitorForQuestion(question);
        }

        public void HandleSignatureCleared(object sender, EventArgs e, Question question)
        {
            HandleQuestionAnswered(question, string.Empty);
   
            SignatureStatus = "Signature cleared";
        }

        private void UpdateContinueButtonText()
        {
            ContinueButtonText = (CurrentStepNumber == Steps?.Count) ? "SUBMIT" : "CONTINUE";
        }
        #endregion
    }
}