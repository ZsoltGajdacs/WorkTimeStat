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
        private double defaultDailyWorkHours;
        private double defaultDailyWorkMinutes;
        private Resolution watcherResolution;
        private DataPrecision watcherDataPrecision;
        private SavePreference watcherSavePreference;
        private WorkPlaceType workPlaceType;
        private AvailableLanguages chosenLanguage;
        private DateTime holidayYearStart;

        public Boolean IsLunchTimeWorkTimeDefault { get => isLunchTimeWorkTimeDefault; set => SetAndNotifyPropertyChanged(ref isLunchTimeWorkTimeDefault, value); }
        public decimal AmountOfLitreInOneUnit { get => amountOfLitreInOneUnit; set => SetAndNotifyPropertyChanged(ref amountOfLitreInOneUnit, value); }
        public double DefaultDailyWorkHours { get => defaultDailyWorkHours; set => SetAndNotifyPropertyChanged(ref defaultDailyWorkHours, value); }
        public double DefaultDailyWorkMinutes { get => defaultDailyWorkMinutes; set => SetAndNotifyPropertyChanged(ref defaultDailyWorkMinutes, value); }
        public Resolution WatcherResolution { get => watcherResolution; set => SetAndNotifyPropertyChanged(ref watcherResolution, value); }
        public DataPrecision WatcherDataPrecision { get => watcherDataPrecision; set => SetAndNotifyPropertyChanged(ref watcherDataPrecision, value); }
        public SavePreference WatcherSavePreference { get => watcherSavePreference; set => SetAndNotifyPropertyChanged(ref watcherSavePreference, value); }
        public WorkPlaceType WorkPlaceType { get => workPlaceType; set => SetAndNotifyPropertyChanged(ref workPlaceType, value); }
        public AvailableLanguages ChosenLanguage { get => chosenLanguage; set => SetAndNotifyPropertyChanged(ref chosenLanguage, value); }
        public DateTime HolidayYearStart { get => holidayYearStart; set => SetAndNotifyPropertyChanged(ref holidayYearStart, value); }
    
        public TimeSpan GetDefaultHours()
        {
            var onlyHoursTime = TimeSpan.FromHours(DefaultDailyWorkHours);
            var onlyMinutes = TimeSpan.FromMinutes(DefaultDailyWorkMinutes);

            return onlyHoursTime.Add(onlyMinutes);
        }
    }
}
