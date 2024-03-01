

namespace UserStore.Domain.Entities
{
    public static class GlobalValue
    {
        public static object Get()
        {
            return new
            {
                AppScreen = new
                {
                    ScreeningTypes = new List<string> { "Target Based", "Phenotypic" },
                    ScreeningMethods = new List<string> { "HTS", "HCS", "Secondary", "Fragment", "Biophysical", "Virtual", "Other" },
                }
            };
        }
    }

}