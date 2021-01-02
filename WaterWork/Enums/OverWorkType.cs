using System.ComponentModel;

namespace WaterWork.Enums
{
    internal enum OverWorkType
    {
        [Description("-")]
        NINCS,

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
