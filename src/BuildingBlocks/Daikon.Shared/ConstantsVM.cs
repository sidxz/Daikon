
using Daikon.Shared.Constants.AppScreen;

namespace Daikon.Shared
{
    public class ConstantsVM
    {
        public static object Get()
        {
            return new
            {
                AppScreen = new
                {
                    ScreeningTypes = ScreeningType.GetScreeningTypes(),

                    ScreeningMethods = ScreeningMethod.GetScreeningMethods(),

                    VotingValues = VotingValue.GetVotingValues()
                }
            };
        }
    }
}