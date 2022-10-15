namespace Emaratech.Services.eVisa.Lookups
{
    internal interface ILookup
    {
        string GetEn(string argId);
        string GetAr(string argId);
    }
}
