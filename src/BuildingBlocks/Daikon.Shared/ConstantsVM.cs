
using Daikon.Shared.Constants.AppHitAssessment;
using Daikon.Shared.Constants.AppProject;
using Daikon.Shared.Constants.AppScreen;
using Daikon.Shared.Constants.AppTarget;
using Daikon.Shared.Constants.Workflow;

namespace Daikon.Shared
{
    public sealed class AppVersionInfo
    {
        public string Version { get; init; } = default!;
        public string Name { get; init; } = default!;
    }
    public class ConstantsVM
    {
        public static AppVersionInfo AppVersion => new()
        {
            Version = "3.1.0",
            Name = "Sintra"
        };
        public static object Get()
        {
            return new
            {
                AppVersion,
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
                },
                AppWorkflow = new
                {
                    Stages = Stages.GetStages()
                }
            };
        }
    }
}