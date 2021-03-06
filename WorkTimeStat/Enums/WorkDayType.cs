using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WorkTimeStat.Enums
{
    internal enum WorkDayType
    {
        [Description("enum_weekday_desc")]
        [Display(Name = "enum_weekday_display")]
        WEEKDAY,

        [Description("enum_extraordinary_desc")]
        [Display(Name = "enum_extraordinary_display")]
        EXTRAORDINARY,
        
        [Description("enum_standby_desc")]
        [Display(Name = "enum_standby_display")]
        STANDBY,
        
        [Description("enum_weekend_desc")]
        [Display(Name = "enum_weekend_display")]
        WEEKEND,
        
        [Description("enum_holiday_desc")]
        [Display(Name = "enum_holiday_display")]
        HOLIDAY
    }
}
