using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WorkTimeStat.Enums
{
    internal enum WorkPlaceType
    {
        [Description("enum_office_day_desc")]
        [Display(Name = "enum_office_day_display")]
        OFFICE,
        [Description("enum_home_office_desc")]
        [Display(Name = "enum_home_office_display")]
        HOME,
        [Description("enum_customer_desc")]
        [Display(Name = "enum_customer_display")]
        CUSTOMER,
        [Description("enum_mixed_desc")]
        [Display(Name = "enum_mixed_display")]
        Mixed,
        [Description("enum_other_desc")]
        [Display(Name = "enum_other_display")]
        OTHER
    }
}
