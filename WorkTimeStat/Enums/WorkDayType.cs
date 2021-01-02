using System.ComponentModel;

namespace WorkTimeStat.Enums
{
    internal enum WorkDayType
    {
        [Description("Normál")]
        NORMAL,

        [Description("Rendkívüli")]
        RENDKIVULI,
        
        [Description("Készenlét")]
        KESZENLET,
        
        [Description("Hétvégi")]
        HETVEGI,
        
        [Description("Ünnepnapi")]
        UNNEPNAPI
    }
}
