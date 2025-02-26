
using Daikon.Shared.Constants.AppHitAssessment;
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
                AppVersion = new
                {
                    Version = "2.5.0",
                    Name = "Valparaíso"
                },
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
                AppHitAssessment = new
                {
                    HitAssessmentStatuses = HitAssessmentStatus.GetHAStatuses()
                },
                AppProject = new
                {
                    ProjectStages = ProjectStage.GetProjectStages()
                }

            };
        }
    }
}