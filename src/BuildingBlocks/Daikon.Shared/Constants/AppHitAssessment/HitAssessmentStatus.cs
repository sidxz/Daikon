using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Daikon.Shared.DataStructures;

namespace Daikon.Shared.Constants.AppHitAssessment
{
    public class HitAssessmentStatus
    {
        public const string ReadyForHA = "Ready for HA";
        public const string Active = "Active";
        public const string IncorrectMz = "Incorrect m/z";
        public const string KnownLiability = "Known Liability";
        public const string CompleteFailed = "Complete - Failed";
        public const string CompleteSuccess = "Complete - Success";
        public const string Paused = "Paused";




        public static List<NameValuePair> GetHAStatuses()
        {
            return [.. new List<NameValuePair>
            {
                new() { Name = ReadyForHA, Value = nameof(ReadyForHA) },
                new() { Name = Active, Value = nameof(Active) },
                new() { Name = IncorrectMz, Value = nameof(IncorrectMz) },
                new() { Name = KnownLiability, Value = nameof(KnownLiability) },
                new() { Name = CompleteFailed, Value = nameof(CompleteFailed) },
                new() { Name = CompleteSuccess, Value = nameof(CompleteSuccess) },
                new() { Name = Paused, Value = nameof(Paused) }
            }];
        }


        // public static string GetStatusNameByValue(string value)
        // {
        //     Type type = typeof(HitAssessmentStatus);
        //     foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
        //     {
        //         if (field.IsLiteral && !field.IsInitOnly && field.FieldType == typeof(string))
        //         {
        //             if (field.GetValue(null).ToString() == value)
        //             {
        //                 return field.Name;
        //             }
        //             continue;
        //         }
        //     }
        //     return null;
        // }
    }
}