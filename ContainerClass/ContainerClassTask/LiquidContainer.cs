namespace ContainerClassTask
{
    public class LiquidContainer : Container, IHazardNotifier
    {
        public LiquidContainer(string serialNumber, double cargoMass, double height, double tareWeight, double depth, double maxPayload)
            : base(serialNumber, cargoMass, height, tareWeight, depth, maxPayload) { }
        public override void LoadCargo(double mass)
        {
            
            if(mass > (MaxPayload*0,9) )
            {
                throw new OverfillException("Cargo mass exceeds maximum payload");
            }

           CargoMass = mass;
        }
    }
}
