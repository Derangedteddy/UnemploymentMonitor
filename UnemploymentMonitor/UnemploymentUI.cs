using Game.UI;
using UnityEngine.Scripting;
using Colossal.UI.Binding;
using Unity.Collections;
using Game.Simulation;
using Unity.Jobs;
using HookUILib.Core;
using Colossal.Collections;
using Unity.Entities;
using Game.Citizens;
using Game.Common;
using Game.Tools;
using Game.Companies;
using Game;
using Unity.Burst.Intrinsics;
using Unity.Burst;
using Game.Modding;

namespace UnemploymentMonitor
{
    public class UnemploymentUI : UIExtension
    {
        public new readonly string extensionID = "derangedteddy.unemployment-monitor";

        public new readonly string extensionContent;

        public new readonly ExtensionType extensionType = ExtensionType.Panel;

        public UnemploymentUI()
        {
            extensionContent = LoadEmbeddedResource("UnemploymentMonitor.unemployment_monitor.transpiled.js");
        }
    }

    public class UnemploymentUISystem : UISystemBase
    {
        private const string kGroup = "unemploymentInfo";

        private CountEmploymentSystem m_CountEmploymentSystem;
        private UnderemploymentSystem m_UnderemploymentSystem;

        NativeArray<int> m_Results = new NativeArray<int>(5, Allocator.Persistent);
        EntityQuery m_HomelessHouseholdQuery;
        EntityQuery m_EmployeesQuery;

        [Preserve]
        protected override void OnCreate()
        {
            m_CountEmploymentSystem = base.World.GetOrCreateSystemManaged<CountEmploymentSystem>();
            m_UnderemploymentSystem = base.World.GetOrCreateSystemManaged<UnderemploymentSystem>();

            base.OnCreate();

            AddUpdateBinding(new GetterValueBinding<int>("unemploymentInfo", "unemploymentTotal", () => m_CountEmploymentSystem.GetUnemployment(out JobHandle deps).value));

            AddUpdateBinding(new GetterValueBinding<int>("unemploymentInfo", "unemploymentEducation0", () => m_CountEmploymentSystem.GetUnemploymentByEducation(out JobHandle deps)[0]));
            AddUpdateBinding(new GetterValueBinding<int>("unemploymentInfo", "unemploymentEducation1", () => m_CountEmploymentSystem.GetUnemploymentByEducation(out JobHandle deps)[1]));
            AddUpdateBinding(new GetterValueBinding<int>("unemploymentInfo", "unemploymentEducation2", () => m_CountEmploymentSystem.GetUnemploymentByEducation(out JobHandle deps)[2]));
            AddUpdateBinding(new GetterValueBinding<int>("unemploymentInfo", "unemploymentEducation3", () => m_CountEmploymentSystem.GetUnemploymentByEducation(out JobHandle deps)[3]));
            AddUpdateBinding(new GetterValueBinding<int>("unemploymentInfo", "unemploymentEducation4", () => m_CountEmploymentSystem.GetUnemploymentByEducation(out JobHandle deps)[4]));

            EntityQueryDesc homelessHouseholdsQueryDesc = new EntityQueryDesc
            {
                All = new ComponentType[2]
                {
                    ComponentType.ReadOnly<Household>(),
                    ComponentType.ReadOnly<HomelessHousehold>()
                },
                None = new ComponentType[5]
                {
                    ComponentType.ReadOnly<Deleted>(),
                    ComponentType.ReadOnly<Destroyed>(),
                    ComponentType.ReadOnly<Temp>(),
                    ComponentType.ReadOnly<TouristHousehold>(),
                    ComponentType.ReadOnly<CommuterHousehold>()
                }
            };

            EntityQueryDesc underemployedEmployeesQueryDesc = new EntityQueryDesc
            {
                All = new ComponentType[2] {
                    ComponentType.ReadOnly<Employee>(),
                    ComponentType.ReadOnly<Worker>()
                },
                None = new ComponentType[3]
                {
                    ComponentType.ReadOnly<Deleted>(),
                    ComponentType.ReadOnly<Destroyed>(),
                    ComponentType.ReadOnly<Temp>()
                }
            };
            
            m_HomelessHouseholdQuery = GetEntityQuery(homelessHouseholdsQueryDesc);
            m_EmployeesQuery = GetEntityQuery(underemployedEmployeesQueryDesc);

            AddUpdateBinding(new GetterValueBinding<int>("unemploymentInfo", "homelessHouseholdCount", () => m_HomelessHouseholdQuery.CalculateEntityCount()));
            AddUpdateBinding(new GetterValueBinding<int>("unemploymentInfo", "underemployedCimsCount", () => m_UnderemploymentSystem.GetUnderemployedCount()));
        }

        [Preserve]
        protected override void OnDestroy()
        {
            m_Results.Dispose();
            base.OnDestroy();
        }
    }

    
}
