using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WorkTimeStat.Enums
{
    internal enum AvailableLanguages
    {
        [Description("en")]
        [Display(Name = "lang_en")]
        ENGLISH,
        [Description("hu_HU")]
        [Display(Name = "lang_hu_HU")]
        HUNGARIAN
    }
}
