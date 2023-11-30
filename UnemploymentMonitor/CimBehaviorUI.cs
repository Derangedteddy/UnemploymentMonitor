using Game.UI;
using UnityEngine.Scripting;
using Colossal.UI.Binding;
using Unity.Collections;
using Game.Simulation;
using Unity.Jobs;
using HookUILib.Core;

namespace UnemploymentMonitor
{
    public class CimBehaviorUI : UIExtension
    {
        public new readonly string extensionID = "derangedteddy.unemployment-monitor";

        public new readonly string extensionContent;

        public new readonly ExtensionType extensionType = ExtensionType.Panel;

        public CimBehaviorUI()
        {
            extensionContent = LoadEmbeddedResource("UnemploymentMonitor.unemployment_monitor.transpiled.js");
        }
    }

    public class CimBehaviorUISystem : UISystemBase
    {
        private const string kGroup = "unemploymentInfo";

        private CountEmploymentSystem m_CountEmploymentSystem;

        NativeArray<int> m_Results = new NativeArray<int>(5, Allocator.Persistent);

        [Preserve]
        protected override void OnCreate()
        {
            m_CountEmploymentSystem = base.World.GetOrCreateSystemManaged<CountEmploymentSystem>();
            
            base.OnCreate();

            AddUpdateBinding(new GetterValueBinding<int>("unemploymentInfo", "unemploymentEducation0", () => m_CountEmploymentSystem.GetUnemploymentByEducation(out JobHandle deps)[0]));
            AddUpdateBinding(new GetterValueBinding<int>("unemploymentInfo", "unemploymentEducation1", () => m_CountEmploymentSystem.GetUnemploymentByEducation(out JobHandle deps)[1]));
            AddUpdateBinding(new GetterValueBinding<int>("unemploymentInfo", "unemploymentEducation2", () => m_CountEmploymentSystem.GetUnemploymentByEducation(out JobHandle deps)[2]));
            AddUpdateBinding(new GetterValueBinding<int>("unemploymentInfo", "unemploymentEducation3", () => m_CountEmploymentSystem.GetUnemploymentByEducation(out JobHandle deps)[3]));
            AddUpdateBinding(new GetterValueBinding<int>("unemploymentInfo", "unemploymentEducation4", () => m_CountEmploymentSystem.GetUnemploymentByEducation(out JobHandle deps)[4]));         
        }

        [Preserve]
        protected override void OnDestroy()
        {
            m_Results.Dispose();
            base.OnDestroy();
        }
    }
}
