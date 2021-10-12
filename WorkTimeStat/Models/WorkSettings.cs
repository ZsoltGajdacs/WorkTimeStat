using Newtonsoft.Json;
using System;
using UsageWatcher.Enums;
using WorkTimeStat.Enums;
using ZsGUtils.UIHelpers;

namespace WorkTimeStat.Models
{
    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    internal class WorkSettings : BindableClass
    {
        private bool isLunchTimeWorkTimeDefault;
        private decimal amountOfLitreInOneUnit;
        private int yearlyLeaveNumber;
        private double dailyWorkHours;
        private Resolution watcherResolution;
        private DataPrecision watcherDataPrecision;
        private SavePreference watcherSavePreference;
        private WorkPlaceType workPlaceType;
        private AvailableLanguages chosenLanguage;
        private DateTime holidayYearStart;

        public Boolean IsLunchTimeWorkTimeDefault { get => isLunchTimeWorkTimeDefault; set => SetAndNotifyPropertyChanged(ref isLunchTimeWorkTimeDefault, value); }
        public decimal AmountOfLitreInOneUnit { get => amountOfLitreInOneUnit; set => SetAndNotifyPropertyChanged(ref amountOfLitreInOneUnit, value); }
        public int YearlyLeaveNumber { get => yearlyLeaveNumber; set => SetAndNotifyPropertyChanged(ref yearlyLeaveNumber, value); }
        public double DailyWorkHours { get => dailyWorkHours; set => SetAndNotifyPropertyChanged(ref dailyWorkHours, value); }
        public Resolution WatcherResolution { get => watcherResolution; set => SetAndNotifyPropertyChanged(ref watcherResolution, value); }
        public DataPrecision WatcherDataPrecision { get => watcherDataPrecision; set => SetAndNotifyPropertyChanged(ref watcherDataPrecision, value); }
        public SavePreference WatcherSavePreference { get => watcherSavePreference; set => SetAndNotifyPropertyChanged(ref watcherSavePreference, value); }
        public WorkPlaceType WorkPlaceType { get => workPlaceType; set => SetAndNotifyPropertyChanged(ref workPlaceType, value); }
        public AvailableLanguages ChosenLanguage { get => chosenLanguage; set => SetAndNotifyPropertyChanged(ref chosenLanguage, value); }
        public DateTime HolidayYearStart { get => holidayYearStart; set => SetAndNotifyPropertyChanged(ref holidayYearStart, value); }
    }
}
