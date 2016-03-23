using System;

namespace BeanAccReaderApp.Model
{
    // Enum holding supported device types.
    public enum DeviceType
    {
		// BloodPressure, HealthThermometer, WeightScale, LightBlueBeanを追加、中島
        GenericAccess, Battery, HeartRate, BloodPressure, HealthThermometer, WeightScale, LightBlueBean
    }

    // This allows the UI to display a name along side a Guid and a type.
    public class NamedGuid
    {
        public string Name { get; set; }

        public Guid Guid { get; set; }

        public DeviceType Type { get; set; }
    }
}
