using Microsoft.AspNetCore.Routing.Constraints;
using System.Linq.Expressions;

namespace ContainerClassTask
{
    public abstract class Container
    {
        public String SerialNumber { get; }
        public double CargoMass { get; protected set; }
        public double Height { get; }
        public double TareWeight { get; }
        public double Depth { get; }
        public double MaxPayload { get; }
        private static int Id = 0;
        protected Container(string serialNumber, double cargoMass, double height, double tareWeight, double depth, double maxPayload)
        {
            SerialNumber = serialNumber;
            CargoMass = cargoMass;
            Height = height;
            TareWeight = tareWeight;
            Depth = depth;
            MaxPayload = maxPayload;
        }

        public abstract void LoadCargo (double mass);
        public abstract void EmptyCargo ();
        private static string GenerateSerialNumber(string Type)
        {
            Id = Id++;
            string TypeChar = "CC";
            switch (Type)
            {
                case "Liquid":
                    TypeChar = "L";
                    break;
                case "Gas":
                    TypeChar = "G";
                    break;
                case "Refridgerated":
                    TypeChar = "C";
                    break;
            }
            return "KON" + TypeChar + Id;
        }
    }
}
