using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WorkTimeStat.Enums
{
    internal enum WorkDayType
    {
        [Description("Normál hétköznapi munkanap")]
        [Display(Name = "Normál")]
        NORMAL,

        [Description("Rendkívüli munkavégzés")]
        [Display(Name = "Rendkívüli")]
        RENDKIVULI,
        
        [Description("Készenlétben töltött idő")]
        [Display(Name = "Készenlét")]
        KESZENLET,
        
        [Description("Hétvégi munkavégzés")]
        [Display(Name = "Hétvégi")]
        HETVEGI,
        
        [Description("Ünnepnapi munkavégzés")]
        [Display(Name = "Ünnepnapi")]
        UNNEPNAPI
    }
}
