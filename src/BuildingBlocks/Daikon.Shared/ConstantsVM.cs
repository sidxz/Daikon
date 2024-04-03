
using Daikon.Shared.Constants.AppProject;
using Daikon.Shared.Constants.AppScreen;
using Daikon.Shared.Constants.AppTarget;

namespace Daikon.Shared
{
    public class ConstantsVM
    {
        public static object Get()
        {
            return new
            {
                AppTarget = new
                {
                    TargetTypes = TargetType.GetTargetTypes()
                },
                AppScreen = new
                {
                    ScreeningTypes = ScreeningType.GetScreeningTypes(),

                    ScreeningMethods = ScreeningMethod.GetScreeningMethods(),

                    VotingValues = VotingValue.GetVotingValues()
                },
                AppProject = new
                {
                    ProjectStages = ProjectStage.GetProjectStages()
                }

            };
        }
    }
}